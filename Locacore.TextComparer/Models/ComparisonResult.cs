using System;
using System.Collections.Generic;

namespace Locacore.TextComparer
{
    public enum ComparisonResultType
    {
        Equals,
        Different,
        Deletion,
        Addition
    }

    public class ComparisonResult
    {
        public ComparisonResultType ComparisonType { get; private set; }
        public String Text1 { get; private set; }
        public String Text2 { get; private set; }

        public ComparisonResult(ComparisonResultType comparisonType, String text1, String text2)
        {
            this.ComparisonType = comparisonType;
            this.Text1 = text1;
            this.Text2 = text2;
        }

        internal static List<ComparisonResult> FixWronglyDeclaredDifferentAreas(List<ComparisonResult> results)
        {
            for (int index = 0; index < results.Count; index++)
            {
                if (results[index].ComparisonType == ComparisonResultType.Different)
                {
                    if (results[index].Text1.Length <= 0)
                    {
                        results[index].ComparisonType = ComparisonResultType.Addition;
                    }
                    else if (results[index].Text2.Length <= 0)
                    {
                        results[index].ComparisonType = ComparisonResultType.Deletion;
                    }
                }
            }
            return results;
        }

        internal static List<ComparisonResult> CleanComparisonResults(List<ComparisonResult> results, int minRangeLength)
        {
            for (int index = 0; index < results.Count; index++)
            {
                // Check whether this segment falls under the minRangeLength criteria
                // It only has to be checked for "equal" segments, as "different" segments
                // are already different (not matter how long) and additions/deletions can not
                // be aggregated.

                if ((results[index].ComparisonType == ComparisonResultType.Equals) && (results[index].Text1.Length < minRangeLength))
                {
                    int cleanUpIndex = index - 1;
                    while ((cleanUpIndex >= 1) && (results[cleanUpIndex].ComparisonType != ComparisonResultType.Equals) && (results[cleanUpIndex].ComparisonType != ComparisonResultType.Different))
                    {
                        // If the left of the initial segment there is an addition/deletion, make it a "different" segment.
                        // This is because on the right side of the initial segment, there is not an equal segment 
                        // (it can only be a deleted/added/different segment).

                        results[cleanUpIndex].ComparisonType = ComparisonResultType.Different;
                        cleanUpIndex--;
                    }
                    cleanUpIndex = index + 1;
                    while ((cleanUpIndex < results.Count - 1) && (results[cleanUpIndex].ComparisonType != ComparisonResultType.Equals) && (results[cleanUpIndex].ComparisonType != ComparisonResultType.Different))
                    {
                        // If the right of the initial segment there is an addition/deletion, make it a "different" segment.
                        // This is because on the left side of the initial segment, there is not an equal segment 
                        // (it can only be a deleted/added/different segment).

                        results[cleanUpIndex].ComparisonType = ComparisonResultType.Different;
                        cleanUpIndex++;
                    }

                    // Now we have flagged the segments left and right as different. In order to aggregate them
                    // (in a later step), we also set the current segment (type Equal) to type Different.
                    
                    results[index].ComparisonType = ComparisonResultType.Different;
                }
            }

            return results;
        }

        internal static List<ComparisonResult> MergeResults(List<ComparisonResult> results)
        {
            List<ComparisonResult> mergedResults = new List<ComparisonResult>();
            if (results.Count > 0)
            {
                var text1 = results[0].Text1;
                var text2 = results[0].Text2;
                var lastType = results[0].ComparisonType;

                for (int index = 1; index < results.Count; index++)
                {
                    if (results[index].ComparisonType == lastType)
                    {
                        text1 += results[index].Text1;
                        text2 += results[index].Text2;
                    }
                    else
                    {
                        if ((text1.Length > 0) || (text2.Length > 0))
                        {
                            mergedResults.Add(new ComparisonResult(lastType, text1, text2));
                        }
                        text1 = results[index].Text1;
                        text2 = results[index].Text2;
                        lastType = results[index].ComparisonType;
                    }
                }
                if ((text1.Length > 0) || (text2.Length > 0))
                {
                    mergedResults.Add(new ComparisonResult(lastType, text1, text2));
                }
            }

            return mergedResults;
        }
    }
}