using System;
using System.Collections.Generic;
using System.Linq;

namespace Locacore.TextComparer
{
    internal class DetailedComparer
    {
        private static int[,] CalculateWagnerFisherLevenshteinDistanceMatrix(string text1, string text2)
        {
            int[,] matrix = new int[text1.Length + 1, text2.Length + 1];

            for (int x = 1; x <= text1.Length; x++)
            {
                matrix[x, 0] = x;
            }

            for (int y = 1; y <= text2.Length; y++)
            {
                matrix[0, y] = y;
            }

            int substcost = 0;
            for (int y = 1; y <= text2.Length; y++)
            {
                for (int x = 1; x <= text1.Length; x++)
                {
                    substcost = (text1[x - 1] == text2[y - 1]) ? 0 : 1;
                    matrix[x, y] = Math.Min(Math.Min(matrix[x - 1, y] + 1, matrix[x, y - 1] + 1), matrix[x - 1, y - 1] + substcost);
                }
            }

            return matrix;
        }

        private static List<ComparisonResult> DetermineDifferences(string text1, string text2)
        {
            List<ComparisonResult> results = new List<ComparisonResult>();

            if (((long)text1.Length) * ((long)text2.Length) > 1_000_000)
            {
                // This is a kind of emergency exist. It means that we would otherwise
                // calculate the Wagner-Fisher-Levenshtein Distance using a very large
                // array - we would not like to do that! Just mark the whole block as
                // different, we can't do anything wrong by that (better mark it as different
                // while being (partly) equal than the other way round).
                // Anyway, this should happen only in very rare instances (when the
                // algorithmns before failed). This happens eg. when just copying the same text
                // line after and after - in that case the algorithmn can't assign a position
                // in one text to the appropriate position in the second text.

                results.Add(new ComparisonResult(ComparisonResultType.Different, text1, text2));
                return results;
            }

            var matrix = CalculateWagnerFisherLevenshteinDistanceMatrix(text1, text2);

            var sx = text1.Length;
            var sy = text2.Length;

            while ((sx >= 1) || (sy >= 1))
            {
                // If we reached the left side of the matrix, the only way to
                // go is upward

                if (sx <= 0)
                {
                    results.Add(new ComparisonResult(ComparisonResultType.Addition, "", text2[sy - 1].ToString()));
                    sy--;
                }

                // If we reached the top side of the matrix, the only way to
                // go is left

                else if (sy <= 0)
                {
                    results.Add(new ComparisonResult(ComparisonResultType.Deletion, text1[sx - 1].ToString(), ""));
                    sx--;
                }

                // Otherwise, prefer a diagonal step to the left-top corner
                // of the matrix

                else if ((matrix[sx - 1, sy - 1] <= matrix[sx, sy - 1]))
                {
                    if (matrix[sx - 1, sy - 1] <= matrix[sx - 1, sy])  // Prefer -1,-1
                    {
                        if (text1[sx - 1] == text2[sy - 1])
                        {
                            results.Add(new ComparisonResult(ComparisonResultType.Equals, text1[sx - 1].ToString(), text2[sy - 1].ToString()));
                        }
                        else
                        {
                            results.Add(new ComparisonResult(ComparisonResultType.Different, text1[sx - 1].ToString(), text2[sy - 1].ToString()));
                        }
                        sx--;
                        sy--;
                    }
                    else // matrix[sx-1, sy] < matrix[sx-1, sy-1] < matrix[sx, sy-1]
                    {
                        results.Add(new ComparisonResult(ComparisonResultType.Deletion, text1[sx - 1].ToString(), ""));
                        sx--;
                    }
                }
                else // matrix[sx, sy-1] < matrix[sx-1, sy-1]
                {
                    if (matrix[sx, sy - 1] < matrix[sx - 1, sy])
                    {
                        results.Add(new ComparisonResult(ComparisonResultType.Addition, "", text2[sy - 1].ToString()));
                        sy--;
                    }
                    else // matrix[sx-1, sy] <= matrix[sx, sy-1] <= matrix[sx-1, sy-1]
                    {
                        results.Add(new ComparisonResult(ComparisonResultType.Deletion, text1[sx - 1].ToString(), ""));
                        sx--;
                    }
                }
            }

            // As we go from the bottom-right end of the matrix
            // we need to reverse the character list

            results.Reverse();

            // Now merge the single characters to larger sets
            // of strings

            return results;
        }

        internal static List<ComparisonResult> DetailedCompare(string text1, string text2)
        {
            // In order to speed the comparison up, check whether
            // there is an empty string on either side, then
            // the whole string is an addition or deletion

            if (String.IsNullOrEmpty(text1))
            {
                if (!String.IsNullOrEmpty(text2))
                {
                    return new List<ComparisonResult>() { new ComparisonResult(ComparisonResultType.Addition, "", text2) };
                }
            }
            else if (String.IsNullOrEmpty(text2))
            {
                return new List<ComparisonResult>() { new ComparisonResult(ComparisonResultType.Deletion, text1, "") };
            }
            else
            {
                // Check whether the texts are equal at the beginning and/or end

                var equalityResultAtBegin = GetEqualityComparisonResultAtBeginOfTexts(ref text1, ref text2);
                var equalityResultAtEnd = GetEqualityComparisonResultAtEndOfTexts(ref text1, ref text2);

                // We need to perform a full compare of the two texts

                var results = DetermineDifferences(text1, text2);

                // If they are equal in the beginning and/or end, add the appropriate comparison
                // result to the result list.

                if (equalityResultAtBegin != null)
                {
                    results.Insert(0, equalityResultAtBegin);
                }
                if (equalityResultAtEnd != null)
                {
                    results.Add(equalityResultAtEnd);
                }

                return results;
            }

            // If both texts are null or empty we just return an empty
            // set of changes

            return new List<ComparisonResult>();
        }

        private static ComparisonResult GetEqualityComparisonResultAtBeginOfTexts(ref string text1, ref string text2)
        {
            int index = 0;
            while ((index < text1.Length) && (index < text2.Length))
            {
                if (text1[index] != text2[index]) break;
                index++;
            }
            if (index > 0)
            {
                var result = new ComparisonResult(ComparisonResultType.Equals, text1.Substring(0, index), text2.Substring(0, index));
                text1 = text1.Substring(index);
                text2 = text2.Substring(index);
                return result;
            }
            else
            {
                return null;
            }
        }

        private static ComparisonResult GetEqualityComparisonResultAtEndOfTexts(ref string text1, ref string text2)
        {
            int index1 = text1.Length - 1;
            int index2 = text2.Length - 1;
            bool foundAtLeastOneMatchingCharacter = false;
            while ((index1 >= 0) && (index2 >= 0))
            {
                if (text1[index1] != text2[index2]) break;
                index1--;
                index2--;
                foundAtLeastOneMatchingCharacter = true;
            }
            if (foundAtLeastOneMatchingCharacter)
            {
                var result = new ComparisonResult(ComparisonResultType.Equals, text1.Substring(index1 + 1), text2.Substring(index2 + 1));
                text1 = text1.Substring(0, index1 + 1);
                text2 = text2.Substring(0, index2 + 1);
                return result;
            }
            else
            {
                return null;
            }
        }

        internal static int DetermineTextDistance(string text1, string text2)
        {
            // In order to speed the comparison up, check whether
            // there is an empty string on either side, then
            // the whole string is an addition or deletion

            if (String.IsNullOrEmpty(text1))
            {
                if (!String.IsNullOrEmpty(text2))
                {
                    return int.MaxValue;
                }
            }
            else if (String.IsNullOrEmpty(text2))
            {
                return int.MaxValue;
            }
            else
            {
                // We need to perform a full compare of the two texts

                return DetermineDifferences(text1, text2)
                    .Where(x => x.ComparisonType != ComparisonResultType.Equals)
                    .Count();
            }

            // If both texts are null or empty we just return an empty
            // set of changes

            return 0;
        }
    }
}