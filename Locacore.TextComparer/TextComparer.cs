using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Locacore.TextComparer
{
    public class TextComparer : ITextComparer
    {
        public TextComparer()
        {
        }

        public TextComparerConfiguration TextComparerConfiguration
        {
            get { return this.Configuration; }
        }

        private TextComparerConfiguration Configuration = new TextComparerConfiguration();

        // Most texts are too large to use recursion, so better use a queue

        private Queue<QueueItem> CompareQueue = new Queue<QueueItem>();

        // We use a Fibonacci sized text length for hashing. If there are no
        // matches for the longest text length, then try the smaller one, etc.

        private readonly int[] HashLengths = {144, 89, 55, 34, 21, 13, 8, 5};

        // This will hash all portions of the specified length of a given text
        // and return a dictionary of hashes to lists of positions
        // of matches within the text

        private Dictionary<int, int> HashText(string text, int hashLength)
        {
            var hashPositions = new Dictionary<int, List<int>>();

            var maxPosition = text.Length - hashLength;

            for (int position = 0; position < maxPosition; position++)
            {
                var substring = text.Substring(position, hashLength);

                var hash = substring.GetHashCode();

                if (!hashPositions.ContainsKey(hash))
                {
                    hashPositions[hash] = new List<int>();
                }

                hashPositions[hash].Add(position);
            }

            return hashPositions

                // Group the dictionary by the number of matches in the attached list
                .GroupBy(hashDictionaryItem => hashDictionaryItem.Value.Count)

                // Only use hashes which are unique (meaning that the portion of the 
                // text does not occur again in the whole text)
                .Where(x => x.Key == 1)

                // Create a new dictionary, where a hash has not anymore a list attached
                // but the single item (which we enforced with the last Where clause)
                .Select(x => x.ToDictionary(y => y.Key, y => y.Value.Single()))

                // As we only have at max a single group, take the one (or return
                // null if there is no hash which only occurs once)
                .SingleOrDefault();
        }

        private List<MatchInformation> MergeAreas(List<MatchInformation> allEqualityPositions)
        {
            var orderedPositionsSet = allEqualityPositions
                .OrderBy(positionInfo => positionInfo.PositionInText1)
                .ToArray();

            var mergeResult = new List<MatchInformation>();

            var position = 0;

            while (position < orderedPositionsSet.Length)
            {
                var currentStartPositionInText1 = orderedPositionsSet[position].PositionInText1;
                var currentEndPositionInText1 = currentStartPositionInText1 + orderedPositionsSet[position].Length;
                var currentOffset = orderedPositionsSet[position].Offset;

                // At the end, there will be still some elements which should
                // all be merged into the last item, check if we should merge at the end

                var alreadyAdded = false;

                var nextPositionToCheck = position + 1;

                while (nextPositionToCheck < orderedPositionsSet.Length)
                {
                    // Make sure that the offset hasn't changed, otherwise we can't merge!

                    if ((currentEndPositionInText1 >= orderedPositionsSet[nextPositionToCheck].PositionInText1) &&
                        (currentOffset == orderedPositionsSet[nextPositionToCheck].Offset))
                    {
                        // It's the same, so modify the end position and continue merge

                        currentEndPositionInText1 = orderedPositionsSet[nextPositionToCheck].PositionInText1 +
                                                    orderedPositionsSet[nextPositionToCheck].Length;

                        nextPositionToCheck++;
                    }
                    else
                    {
                        // It is different, so create a (merged) match information and start processing from here

                        mergeResult.Add(new MatchInformation(
                            currentStartPositionInText1,
                            orderedPositionsSet[position].PositionInText2,
                            currentEndPositionInText1 - currentStartPositionInText1));

                        position = nextPositionToCheck;

                        // Do not add this information again

                        alreadyAdded = true;

                        // Break the inner loop, continue with the outer loop

                        break;
                    }
                }

                if (!alreadyAdded)
                {
                    // This is the last data from the inner loop, also process it

                    mergeResult.Add(new MatchInformation(currentStartPositionInText1,
                        orderedPositionsSet[position].PositionInText2,
                        currentEndPositionInText1 - currentStartPositionInText1));
                    position = nextPositionToCheck;
                }
            }

            return mergeResult;
        }

        // Filters the entries of a Dictionary to given positions (limit the hashes to positions
        // between lowerBorder and upperBorder

        private Dictionary<int, int> FilterHashes(Dictionary<int, int> hashPositions, int lowerBorder, int upperBorder)
        {
            var filteredHashPositions = hashPositions
                .Where(hashPosition => (hashPosition.Value >= lowerBorder) && (hashPosition.Value < upperBorder))
                .ToDictionary(hashPosition => hashPosition.Key, x => x.Value - lowerBorder);

            return filteredHashPositions;
        }

        private MatchInformation GetBestMatch(Dictionary<int, int> hashPositionsInText1,
            Dictionary<int, int> hashPositionsInText2, int hashLength)
        {
            var matchResults = new List<MatchInformation>();

            // Check whether we have matches for some of the hashes (which occur only 
            // on a single position in both texts)

            foreach (var hashPositionInText1 in hashPositionsInText1)
            {
                if (hashPositionsInText2.ContainsKey(hashPositionInText1.Key))
                {
                    matchResults.Add(new MatchInformation(hashPositionInText1.Value,
                        hashPositionsInText2[hashPositionInText1.Key], hashLength));
                }
            }

            var mergedBestMatchResults = MergeAreas(matchResults);

            // If we have no result at all, just return null

            if (mergedBestMatchResults.Count <= 0) return null;

            // If we have a single best match, just return it

            if (mergedBestMatchResults.Count == 1) return mergedBestMatchResults.Single();

            // Otherwise use the heuristic, that the largest block of text will
            // be the most correct place to align the texts

            var sortedBestMatches = mergedBestMatchResults.OrderByDescending(x => x.Length);

            var bestResult = sortedBestMatches.FirstOrDefault();

            return bestResult;
        }

        private bool CompareTextsWithFixedHashSize(Dictionary<int, int> hashPositionsInText1,
            Dictionary<int, int> hashPositionsInText2, int currentHashLength, string text1, string text2,
            int relativeOffset1, int relativeOffset2, List<HashComparisonResult> results, bool initial)
        {
            MatchInformation match = null;

            // Only if we have some hashes in each of the dictionaries it makes sense
            // to find a match

            if ((hashPositionsInText1.Count > 0) && (hashPositionsInText2.Count > 0))
            {
                match = GetBestMatch(hashPositionsInText1, hashPositionsInText2, currentHashLength);
            }

            if (match != null)
            {
                var comparisonResult = new ComparisonResult(
                    ComparisonResultType.Equals,
                    text1.Substring(match.PositionInText1, match.Length),
                    text2.Substring(match.PositionInText2, match.Length));

                var matchInformation = new MatchInformation(
                    match.PositionInText1 + relativeOffset1,
                    match.PositionInText2 + relativeOffset2,
                    match.Length);

                results.Add(new HashComparisonResult(matchInformation, comparisonResult));

                // Filter the hashes (thus we don't need to calculate all the hashes again)
                // This is only a heuristic, because within the smaller text most probably more
                // hashpositions would be unique, thus more positions could be found

                var leftHashPositionsInText1 = FilterHashes(hashPositionsInText1,
                    0, match.PositionInText1 - (currentHashLength - 1));

                var leftHashPositionsInText2 = FilterHashes(hashPositionsInText2,
                    0, match.PositionInText2 - (currentHashLength - 1));

                // Recurse into the left side (might work out bad)

                CompareTextsWithFixedHashSize(leftHashPositionsInText1, leftHashPositionsInText2,
                    currentHashLength,
                    text1.Substring(0, match.PositionInText1),
                    text2.Substring(0, match.PositionInText2),
                    relativeOffset1,
                    relativeOffset2,
                    results, false);

                // Same for the right side

                var rightHashPositionsInText1 = FilterHashes(hashPositionsInText1,
                    match.PositionInText1 + match.Length, int.MaxValue);

                var rightHashPositionsInText2 = FilterHashes(hashPositionsInText2,
                    match.PositionInText2 + match.Length, int.MaxValue);

                CompareTextsWithFixedHashSize(rightHashPositionsInText1, rightHashPositionsInText2,
                    currentHashLength,
                    text1.Substring(match.PositionInText1 + match.Length),
                    text2.Substring(match.PositionInText2 + match.Length),
                    match.PositionInText1 + match.Length + relativeOffset1,
                    match.PositionInText2 + match.Length + relativeOffset2,
                    results, false);

                // We found a match

                return true;
            }
            else
            {
                // If this is an initial call, it directly happened from CompareTexts. In that
                // case, don't add a new queue element, because the loop in CompareTexts will
                // choose a smaller text portion for hashing and start over with the texts

                if (!initial)
                {
                    CompareQueue.Enqueue(new QueueItem(
                        text1, text2,
                        relativeOffset1, relativeOffset2,
                        0, results));
                }

                // Nothing found so far

                return false;
            }
        }

        private void CompareTexts(string text1, string text2, int relativeOffset1, int relativeOffset2,
            List<HashComparisonResult> results)
        {
            // Try all text portions of the Fibonacci lengths specified above

            for (int currentHashLengthPosition = 0;
                currentHashLengthPosition < HashLengths.Length;
                currentHashLengthPosition++)
            {
                // Get the current text length for hashing from the definition array

                var currentHashLength = HashLengths[currentHashLengthPosition];

                // If the hash length is larger than any of the texts, directly
                // choose a smaller one, as this will produce no hashes

                if ((currentHashLength > text1.Length) || (currentHashLength > text2.Length))
                {
                    continue;
                }

                // Hash both texts with position information

                var hashPositionsInText1 = HashText(text1, currentHashLength);
                var hashPositionsInText2 = HashText(text2, currentHashLength);

                // If we have at least some entries in each of the dictionaries then
                // compare the hashes (and recursively add new jobs to the queue)

                if ((hashPositionsInText1 != null) && (hashPositionsInText2 != null))
                {
                    if (CompareTextsWithFixedHashSize(
                        hashPositionsInText1, hashPositionsInText2,
                        currentHashLength,
                        text1, text2,
                        relativeOffset1, relativeOffset2,
                        results, true))
                    {
                        // We found a match (and further matches recursively or at least added
                        // some more comparison request into the queue

                        break;
                    }
                }
            }
        }

        private void ProcessQueue()
        {
            while (CompareQueue.Count > 0)
            {
                var item = CompareQueue.Dequeue();

                CompareTexts(
                    item.Text1, item.Text2,
                    item.RelativePosition1, item.RelativePosition2,
                    item.HashResults);
            }
        }


        public List<ComparisonResult> CompareTexts(string text1, string text2)
        {
            var finalResult = new List<ComparisonResult>();

            if ((text1.Length < 5) || (text2.Length < 5) || (((long) text1.Length) * ((long) text2.Length) < 100))
            {
                finalResult =
                    DetailedComparer.DetailedCompare(text1, text2);
                
                finalResult = ComparisonResult.MergeResults(finalResult);
                finalResult = ComparisonResult.CleanComparisonResults(finalResult, Configuration.MinimumSizeForRangesOfDifferentText);
                finalResult = ComparisonResult.MergeResults(finalResult);
            }
            else
            {
                // Create an empty result list

                var hashComparisonResults = new List<HashComparisonResult>();

                // Enqueue the first comparison

                CompareQueue.Enqueue(new QueueItem(text1, text2, 0, 0, 0, hashComparisonResults));

                // Process all enqueued items until the list is empty

                ProcessQueue();

#if DEBUG
                // Check if we can reconstruct the texts from the given information

                foreach (var hashComparisonResult in hashComparisonResults)
                {
                    var matchedText1 = text1.Substring(
                        hashComparisonResult.Position.PositionInText1,
                        hashComparisonResult.Position.Length);

                    var matchedText2 = text2.Substring(
                        hashComparisonResult.Position.PositionInText2,
                        hashComparisonResult.Position.Length);

                    if (matchedText1 != hashComparisonResult.Result.Text1)
                        throw new Exception("Match error");

                    if (matchedText2 != hashComparisonResult.Result.Text2)
                        throw new Exception("Match error");
                }
#endif

                if (!hashComparisonResults.Any())
                {
                    var tc = new TextComparer();
                    var leftText1 = text1.Substring(0, text1.Length / 2);
                    var rightText1 = text1.Substring(text1.Length / 2);
                    var leftText2 = text2.Substring(0, text2.Length / 2);
                    var rightText2 = text2.Substring(text2.Length / 2);

                    var leftResult = tc.CompareTexts(leftText1, leftText2);
                    var rightResult = tc.CompareTexts(rightText1, rightText2);

                    finalResult.AddRange(leftResult);
                    finalResult.AddRange(rightResult);
                }
                else
                {
                    // Sort the results into the order of the reference text (text1)

                    var sortedComparisonResults = hashComparisonResults
                        .OrderBy(result => result.Position.PositionInText1)
                        .ToArray();

                    // Now we need to compare the parts that hasn't been compared
                    // so far (eg. because some parts differ from the other text)

                    var positionInText1 = 0;
                    var positionInText2 = 0;

                    // Loop over all results in order to find the areas which are not equal

                    for (int resultIndex = 0; resultIndex < sortedComparisonResults.Length; resultIndex++)
                    {
                        var resultPosition = sortedComparisonResults[resultIndex].Position;

                        var lengthWithNoMatchInText1 = resultPosition.PositionInText1 - positionInText1;
                        var lengthWithNoMatchInText2 = resultPosition.PositionInText2 - positionInText2;

                        // Are there any texts which are not in the results of equal areas

                        if ((lengthWithNoMatchInText1 > 0) || (lengthWithNoMatchInText2 > 0))
                        {
                            // Start a detailed comparison for those areas

                            var comparisonResult = DetailedComparer.DetailedCompare(
                                text1.Substring(positionInText1, lengthWithNoMatchInText1),
                                text2.Substring(positionInText2, lengthWithNoMatchInText2));

                            // Add the detailed result to the final result set

                            finalResult.AddRange(comparisonResult);
                        }

                        // Proceed to the next equal area to see whether the is some inequality
                        // right behind it

                        positionInText1 = resultPosition.PositionInText1 + resultPosition.Length;
                        positionInText2 = resultPosition.PositionInText2 + resultPosition.Length;

                        // Also add the original result

                        finalResult.Add(sortedComparisonResults[resultIndex].Result);
                    }

                    // Behind the last equal area there might be still an unequality area,
                    // so also check this

                    var remainingLengthInText1 = text1.Length - positionInText1;
                    var remainingLengthInText2 = text2.Length - positionInText2;

                    // Is there still some text remaining

                    if ((remainingLengthInText1 > 0) || (remainingLengthInText2 > 0))
                    {
                        // Again compare it and store it into the final result set

                        var comparisonResult = DetailedComparer.DetailedCompare(
                            text1.Substring(positionInText1, remainingLengthInText1),
                            text2.Substring(positionInText2, remainingLengthInText2));

                        finalResult.AddRange(comparisonResult);
                    }
                }

                finalResult = FixBreakingSourceIntoParts(finalResult);
                finalResult = FixBreakingSourceIntoParts(finalResult);
                
                finalResult = ComparisonResult.MergeResults(finalResult);
                
                finalResult = ComparisonResult.CleanComparisonResults(finalResult, Configuration.MinimumSizeForRangesOfDifferentText);
                finalResult = ComparisonResult.MergeResults(finalResult);

                finalResult = ComparisonResult.FixWronglyDeclaredDifferentAreas(finalResult);
                finalResult = ComparisonResult.MergeResults(finalResult);

                // The results are now already ordered in their occurrence
            }

            // Now we want to have all our additions or deletions aligned
            // with line breaks - or if there is no line break around, align
            // it rightmost

            finalResult = TryToMoveAdditionsAndDeletionsToEitherLineBreakOrRightMost(finalResult);

            if (this.Configuration.MinimumSizeForRangesOfDifferentText > 0)
            {
                // If there is a minimum size for a "different text" 
                // adjacent range is specified, check these ranges and add
                // them to the "different text" range.
                // This helps to remove tiny fragmented ranges that just
                // confuses the user.

                finalResult = ComparisonResult.MergeResults(finalResult);
                
                finalResult = CompactRangesWithDifferencesToLargerFragments(finalResult,
                    TextComparerConfiguration.MinimumSizeForRangesOfDifferentText);
            }

            // Sometimes a new merge is needed as ranges inbetween
            // are removed, so that two ranges can again be merged

            finalResult = ComparisonResult.MergeResults(finalResult);

#if DEBUG
            Debug.Assert(CheckComparerLoss(text1, text2, finalResult));
#endif

            return finalResult;
        }

        private bool CheckComparerLoss(string text1, string text2, List<ComparisonResult> result)
        {
            var text1result = string.Join("", result.Select(x => x.Text1));
            var text2result = string.Join("", result.Select(x => x.Text2));

            if ((text1result != text1) || (text2result != text2))
            {
                return false;
            }

            return true;
        }

        private List<ComparisonResult> FixBreakingSourceIntoParts(List<ComparisonResult> ranges)
        {
            // If we splitted up the source texts (because e.g. no hash match could be found)
            // and compared the parts, there are problems at the intersections.
            // The real solution may be to perform a refined analysis at the intersection
            // points (e.g. using DetailedComparer). But for now, we just see whether
            // there are Added/Deleted or Deleted/Added combinations, which have the same
            // text - this will only occur at such intersection points

            var result = new List<ComparisonResult>();

            var index = 0;

            for (index = 0; index < ranges.Count - 1; index++)
            {
                if ((ranges[index].ComparisonType != ComparisonResultType.Equals) &&
                     (ranges[index + 1].ComparisonType != ComparisonResultType.Equals))
                {
                    var text1combined = ranges[index].Text1 + ranges[index + 1].Text1;
                    var text2combined = ranges[index].Text2 + ranges[index + 1].Text2;

                    // This is rather a cheap test, so try it first
                    if (text1combined == text2combined)
                    {
                        result.Add(new ComparisonResult(ComparisonResultType.Equals, text1combined, text2combined));
                    }
                    else
                    {
                        var combinedComparison = DetailedComparer.DetailedCompare(
                            text1combined,
                            text2combined);

                        result.AddRange(combinedComparison);
                    }

                    // As we don't want to process the next range, just skip it

                    index++;
                }
                else
                {
                    result.Add(ranges[index]);
                }
            }

            if (index < ranges.Count)
            {
                result.Add(ranges[index]);
            }

            return result;
        }

        private List<ComparisonResult> TryToMoveAdditionsAndDeletionsToEitherLineBreakOrRightMost(
            List<ComparisonResult> ranges)
        {
            for (int index = 1; index < ranges.Count - 1; index++)
            {
                // Only adapt the "addition/deleted text" ranges

                if ((ranges[index].ComparisonType == ComparisonResultType.Addition) ||
                    (ranges[index].ComparisonType == ComparisonResultType.Deletion))
                {
                    var allPossibleMovements = new List<(bool foundLineBreak, int offset)>();
                    if (ranges[index - 1].ComparisonType == ComparisonResultType.Equals)
                    {
                        allPossibleMovements.AddRange(FindLineBreakOrMaximumShift(ranges[index], ranges[index - 1],
                            -1));
                    }

                    if (ranges[index + 1].ComparisonType == ComparisonResultType.Equals)
                    {
                        allPossibleMovements.AddRange(FindLineBreakOrMaximumShift(ranges[index], ranges[index + 1],
                            +1));
                    }

                    var best = allPossibleMovements.OrderByDescending(x => x.foundLineBreak)
                        .ThenByDescending(x => x.offset).FirstOrDefault();

                    var text = string.IsNullOrEmpty(ranges[index].Text1) ? ranges[index].Text2 : ranges[index].Text1;

                    if (best.offset < 0)
                    {
                        var cutPoint = ranges[index - 1].Text1.Length + best.offset;
                        var textToBeMovedInsideAddDeleteRange = ranges[index - 1].Text1.Substring(cutPoint);
                        var staysOutsideAddDeleteRange = ranges[index - 1].Text1.Substring(0, cutPoint);
                        text = textToBeMovedInsideAddDeleteRange + text;
                        var textToBeMovedOutside = text.Substring(text.Length + best.offset);
                        var staysInsideAddDeleteRange = text.Substring(0, text.Length + best.offset);

                        ranges[index - 1] = new ComparisonResult(ranges[index - 1].ComparisonType,
                            staysOutsideAddDeleteRange, staysOutsideAddDeleteRange);
                        ranges[index] = new ComparisonResult(ranges[index].ComparisonType,
                            string.IsNullOrEmpty(ranges[index].Text1) ? "" : staysInsideAddDeleteRange,
                            string.IsNullOrEmpty(ranges[index].Text2) ? "" : staysInsideAddDeleteRange);
                        ranges[index + 1] = new ComparisonResult(ranges[index + 1].ComparisonType,
                            textToBeMovedOutside + ranges[index + 1].Text1,
                            textToBeMovedOutside + ranges[index + 1].Text2);
                    }
                    else if (best.offset > 0)
                    {
                        var cutPoint = best.offset;
                        var textToBeMovedInsideAddDeleteRange = ranges[index + 1].Text1.Substring(0, cutPoint);
                        var staysOutsideAddDeleteRange = ranges[index + 1].Text1.Substring(cutPoint);
                        text = text + textToBeMovedInsideAddDeleteRange;
                        var textToBeMovedOutside = text.Substring(0, best.offset);
                        var staysInsideAddDeleteRange = text.Substring(best.offset);

                        ranges[index + 1] = new ComparisonResult(ranges[index + 1].ComparisonType,
                            staysOutsideAddDeleteRange, staysOutsideAddDeleteRange);
                        ranges[index] = new ComparisonResult(ranges[index].ComparisonType,
                            string.IsNullOrEmpty(ranges[index].Text1) ? "" : staysInsideAddDeleteRange,
                            string.IsNullOrEmpty(ranges[index].Text2) ? "" : staysInsideAddDeleteRange);
                        ranges[index - 1] = new ComparisonResult(ranges[index - 1].ComparisonType,
                            ranges[index - 1].Text1 + textToBeMovedOutside,
                            ranges[index - 1].Text2 + textToBeMovedOutside);
                    }
                }
            }

            ranges = ranges.Where(x => (!string.IsNullOrEmpty(x.Text1)) || (!string.IsNullOrEmpty(x.Text2))).ToList();

            return ranges;
        }

        private List<(bool foundLineBreak, int offset)> FindLineBreakOrMaximumShift(ComparisonResult origin,
            ComparisonResult final, int direction)
        {
            var result = new List<(bool foundLineBreak, int offset)>();

            // The origin range is always a deletion or addition, this means either
            // Text1 or Text2 is always empty

            var textOrigin = string.IsNullOrEmpty(origin.Text1) ? origin.Text2 : origin.Text1;
            var textFinal = string.IsNullOrEmpty(origin.Text1) ? final.Text2 : final.Text1;

            var offset = 0;

            // Perhaps it is already at the beginning of a new line?
            // In that case the previous range must end with a newline code 

            if ((direction < 0) && (textFinal.Length > 0))
            {
                if ((textFinal.Last() == '\r') || (textFinal.Last() == '\n'))
                {
                    result.Add((true, 0));
                }
            }

            // First test a special case whether the whole range could be moved to the
            // given direction. This is usually only possible if the textOrigin
            // range consists of all the same character (e.g. whitespace)

            if (textOrigin.Length <= textFinal.Length)
            {
                var textFinalPositionFull = (direction < 0) ? textFinal.Length - textOrigin.Length : 0;
                var additionalOffset = (direction < 0) ? 0 : textOrigin.Length;
                var correctionOffset = (direction < 0) ? textOrigin.Length : 0;
                while (true)
                {
                    if (textFinal.Substring(textFinalPositionFull, textOrigin.Length).Equals(textOrigin))
                    {
                        textFinalPositionFull += direction;

                        result.Add((false, offset - correctionOffset + additionalOffset));

                        if ((textFinalPositionFull < 0) ||
                            (textFinalPositionFull + additionalOffset >= textFinal.Length))
                        {
                            break;
                        }

                        if ((textFinal[textFinalPositionFull + additionalOffset] == '\r') ||
                            (textFinal[textFinalPositionFull] == '\n'))
                        {
                            result.Add((true, offset - correctionOffset + additionalOffset));
                        }

                        offset += direction;
                    }
                    else
                    {
                        result.Add((false, offset != 0 ? offset - direction - correctionOffset + additionalOffset : 0));
                        break;
                    }
                }
            }

            // Now check to move the range char by char

            var textOriginPosition = (direction < 0) ? textOrigin.Length - 1 : 0;
            var textFinalPosition = (direction < 0) ? textFinal.Length - 1 : 0;

            offset = 0;

            while (textOrigin[textOriginPosition] == textFinal[textFinalPosition])
            {
                if ((textOrigin[textOriginPosition] == '\r') || (textOrigin[textOriginPosition] == '\n'))
                {
                    result.Add((true, offset));
                }

                textOriginPosition += direction;
                textFinalPosition += direction;

                result.Add((false, offset));

                if ((textOriginPosition < 0) ||
                    (textFinalPosition < 0) ||
                    (textOriginPosition >= textOrigin.Length) ||
                    (textFinalPosition >= textFinal.Length))
                {
                    break;
                }

                offset += direction;
            }

            return result;
        }

        private List<ComparisonResult> CompactRangesWithDifferencesToLargerFragments(List<ComparisonResult> ranges,
            int minRangeLength)
        {
            // Move over all ranges (to see whether it can be compacted with a neighbor)

            for (int index = 0; index < ranges.Count; index++)
            {
                // Compact only difference ranges

                if (ranges[index].ComparisonType == ComparisonResultType.Different)
                {
                    var downwardsOk = true;
                    var upwardsOk = true;
                    var indexValue = 1;

                    // Check whether a downwards or upwards neighbor can be compacted

                    while (downwardsOk || upwardsOk)
                    {
                        var loopIndex = index - indexValue;

                        // If it is the first range, just stop here

                        if (loopIndex < 0) downwardsOk = false;

                        // If the loop range is either Addition, Deletion or Difference, check whether the minimum compact range has been reached

                        if ((downwardsOk) &&
                            (((ranges[loopIndex].Text1.Length < minRangeLength) &&
                              (ranges[loopIndex].Text2.Length < minRangeLength)) ||
                             (ranges[loopIndex].ComparisonType != ComparisonResultType.Equals)))
                        {
                            // Compact the result to a Difference range (so that there is no small fragmentation of ranges)

                            ranges[index] = new ComparisonResult(ranges[index].ComparisonType,
                                ranges[loopIndex].Text1 + ranges[index].Text1,
                                ranges[loopIndex].Text2 + ranges[index].Text2);

                            // Replace the original comparison range with an empty result (which will be filtered out later on)

                            ranges[loopIndex] = new ComparisonResult(ranges[loopIndex].ComparisonType, "", "");
                        }
                        else
                        {
                            downwardsOk = false;
                        }

                        loopIndex = index + indexValue;

                        // If it is the last range, just stop here

                        if (loopIndex >= ranges.Count) upwardsOk = false;

                        // If the loop range is either Addition, Deletion or Difference, check whether the minimum compact range has been reached

                        if ((upwardsOk) &&
                            (((ranges[loopIndex].Text1.Length < minRangeLength) &&
                              (ranges[loopIndex].Text2.Length < minRangeLength)) ||
                             (ranges[loopIndex].ComparisonType != ComparisonResultType.Equals)))
                        {
                            // Compact the result to a Difference range (so that there is no small fragmentation of ranges)

                            ranges[index] = new ComparisonResult(ranges[index].ComparisonType,
                                ranges[index].Text1 + ranges[loopIndex].Text1,
                                ranges[index].Text2 + ranges[loopIndex].Text2);

                            // Replace the original comparison range with an empty result (which will be filtered out later on)

                            ranges[loopIndex] = new ComparisonResult(ranges[loopIndex].ComparisonType, "", "");
                        }
                        else
                        {
                            upwardsOk = false;
                        }

                        indexValue++;
                    }
                }
            }

            // Remove ranges that have both texts empty (thus have no comparison information)

            ranges = ranges.Where(x => (!string.IsNullOrEmpty(x.Text1)) || (!string.IsNullOrEmpty(x.Text2))).ToList();

            return ranges;
        }
    }
}