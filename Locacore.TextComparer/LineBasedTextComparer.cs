using System;
using System.Collections.Generic;
using System.Linq;

namespace Locacore.TextComparer
{
    public class LineBasedTextComparer : TextComparer, ILineBasedTextComparer
    {
        public List<LineBasedComparisonResult> CompareTextsLineBased(string text1, string text2)
        {
            var compareResult = CompareTexts(text1.Replace("\r",""), text2.Replace("\r",""));

            var lineBasedResultWithAdditionalLineInformation = ConvertResultToLinedBasedResult(compareResult);
            IEnumerable<LineInformation> lineInformationForText1 = ConvertToMappedLineSegments(lineBasedResultWithAdditionalLineInformation, lineResult => lineResult.Text1, lineResult => lineResult.Text2, ShouldLineResultBeConvertedToAdditionTypeForText1);
            IEnumerable<LineInformation> lineInformationForText2 = ConvertToMappedLineSegments(lineBasedResultWithAdditionalLineInformation, lineResult => lineResult.Text2, lineResult => lineResult.Text1, ShouldLineResultBeConvertedToAdditionTypeForText2);

            var lineQueueForText1 = new Queue<LineInformation>(lineInformationForText1);
            var lineQueueForText2 = new Queue<LineInformation>(lineInformationForText2);

            var comparisonResult = new List<LineBasedComparisonResult>();

            while ((lineQueueForText1.Count > 0) || (lineQueueForText2.Count > 0))
            {
                var resultingLineInformationForBothTexts = new LineBasedComparisonResult();
                var upcomingLineInformationForText1 = lineQueueForText1.Count > 0 ? lineQueueForText1.Peek() : null;
                var upcomingLineInformationForText2 = lineQueueForText2.Count > 0 ? lineQueueForText2.Peek() : null;

                if ((upcomingLineInformationForText1 != null) &&
                    (upcomingLineInformationForText1.SmallerThan(upcomingLineInformationForText2)))
                {
                    // Text 1 should be displayed next (as there is no Text 2 information anymore or it compares to a later line)

                    upcomingLineInformationForText1.ReevaluteAggregatedResultTypeWhenComparedValueIsNull();
                    resultingLineInformationForBothTexts.LineOfText1 = upcomingLineInformationForText1;
                    lineQueueForText1.Dequeue();
                }
                else if ((upcomingLineInformationForText2 != null) &&
                         (upcomingLineInformationForText2.SmallerThan(upcomingLineInformationForText1)))
                {
                    // Text 2 should be displayed next (either there is no Text 2 information anymore or it compares to a later line)

                    upcomingLineInformationForText2.ReevaluteAggregatedResultTypeWhenComparedValueIsNull();
                    resultingLineInformationForBothTexts.LineOfText2 = upcomingLineInformationForText2;

                    lineQueueForText2.Dequeue();
                }
                else
                {
                    // Both Text1 and Text2 should be displayed within one line (as they compare against each other)

                    resultingLineInformationForBothTexts.LineOfText1 = upcomingLineInformationForText1;
                    resultingLineInformationForBothTexts.LineOfText2 = upcomingLineInformationForText2;

                    if (upcomingLineInformationForText2 != null) lineQueueForText2.Dequeue();
                    if (upcomingLineInformationForText1 != null) lineQueueForText1.Dequeue();
                }
                comparisonResult.Add(resultingLineInformationForBothTexts);
            }

            return comparisonResult;
        }

        private bool ShouldLineResultBeConvertedToAdditionTypeForText1(LineBasedComparisonResultInternal lineResult, bool otherTextLineIsEmpty)
        {
            if (lineResult.ComparisonType == ComparisonResultType.Deletion)
            {
                return true;
            }
            else
            {
                if ((lineResult.ComparisonType == ComparisonResultType.Different) && otherTextLineIsEmpty)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShouldLineResultBeConvertedToAdditionTypeForText2(LineBasedComparisonResultInternal lineResult, bool otherTextLineIsEmpty)
        {
            if ((lineResult.ComparisonType == ComparisonResultType.Different) && otherTextLineIsEmpty)
            {
                return true;
            }
            return false;
        }

        private static IEnumerable<LineInformation> ConvertToMappedLineSegments(TextComparisonResult lineBasedResultWithAdditionalLineInformation, 
            Func<LineBasedComparisonResultInternal, TextLines[]> fetchCurrentText, 
            Func<LineBasedComparisonResultInternal, TextLines[]> fetchComparisonText,
            Func<LineBasedComparisonResultInternal, bool, bool> resultShouldBeMappedToAdditionResult)
        {
            var listOfLineInformation = lineBasedResultWithAdditionalLineInformation
                    .LineBasedComparisonResults
                    .SelectMany((lineResult, lineIndex) =>
                    {
                        var otherTextLineIsEmpty = !fetchComparisonText(lineResult).Any(otherTextSegment => !string.IsNullOrEmpty(otherTextSegment.Text));

                        var mappedLineSegments = fetchCurrentText(lineResult)
                            // Only use texts that are not empty and which are no additions (which means that text1 should be empty anyway)
                            .Where(textSegment => (!string.IsNullOrEmpty(textSegment.Text)) || 
                                                  ((lineResult.ComparisonType != ComparisonResultType.Addition) && (lineResult.ComparisonType != ComparisonResultType.Equals)))
                            // Create new segment information which maps the comparison type for each text segment
                            .Select((textSegment, textSegmentIndex) =>
                            {
                                var mappedComparisonType = lineResult.ComparisonType;

                                if (resultShouldBeMappedToAdditionResult(lineResult, otherTextLineIsEmpty))
                                    mappedComparisonType = ComparisonResultType.Addition;

                                return new LineSegment(
                                    textSegment.Text,
                                    mappedComparisonType,
                                    textSegment.LineNumber,
                                    lineIndex,
                                    textSegmentIndex);
                            });

                        return mappedLineSegments;
                    });

            var lineInformationForText = listOfLineInformation
                .GroupBy(lineInformation => lineInformation.LineNumber)
                .Select(lineInformationGroupedByLineNumber => new LineInformation(lineInformationGroupedByLineNumber.Key, lineInformationGroupedByLineNumber.OrderBy(lineInformation => lineInformation.BlockNumber).ThenBy(lineInformation => lineInformation.BlockLineNumber).ToArray()));
            return lineInformationForText;
        }

        private TextComparisonResult ConvertResultToLinedBasedResult(IEnumerable<ComparisonResult> compareResult)
        {
            var text1LineNumber = 1;
            var text2LineNumber = 1;

            var result = new List<LineBasedComparisonResultInternal>();

            foreach (var singleResult in compareResult)
            {
                var textLines1 = ConvertSingleResultToLinedBasedResult(singleResult.Text1, ref text1LineNumber);
                var textLines2 = ConvertSingleResultToLinedBasedResult(singleResult.Text2, ref text2LineNumber);
                result.Add(new LineBasedComparisonResultInternal() { ComparisonType = singleResult.ComparisonType, Text1 = textLines1, Text2 = textLines2 });
            }

            return new TextComparisonResult()
            {
                LineBasedComparisonResults = result.ToArray()
            };
        }

        private AdditionalLineInformation[] CreateAdditionalLineInformation(IEnumerable<LineBasedComparisonResultInternal> result, Func<LineBasedComparisonResultInternal, TextLines[]> accessor)
        {
            var additionalLineInformation = result
                .SelectMany(lineResult => accessor(lineResult).Select(lineSegment => new { lineSegment.LineNumber, lineResult.ComparisonType }))
                .GroupBy(lineResult => lineResult.LineNumber)
                .Where(groupedLineResults => groupedLineResults.Count() > 1)
                .Select(groupedLineResults => new AdditionalLineInformation()
                {
                    LineNumber = groupedLineResults.Key,
                    Different = groupedLineResults.Any(y => y.ComparisonType == ComparisonResultType.Different),
                    Addition = groupedLineResults.Any(y => y.ComparisonType == ComparisonResultType.Addition),
                    Deletion = groupedLineResults.Any(y => y.ComparisonType == ComparisonResultType.Deletion)
                })
                .ToArray();

            return additionalLineInformation;
        }

        private TextLines[] ConvertSingleResultToLinedBasedResult(string text, ref int firstLineNumber)
        {
            var firstLineNumberInResult = firstLineNumber;
            var lines = text.Split("\n".ToCharArray(), StringSplitOptions.None);
            var result = lines.Select((line, index) => new TextLines() { LineNumber = firstLineNumberInResult + index, Text = line }).ToArray();
            firstLineNumber = result.Max(line => line.LineNumber);
            return result;
        }

        public class AdditionalLineInformation
        {
            public int LineNumber { get; set; }
            public bool Different { get; set; }
            public bool Addition { get; set; }
            public bool Deletion { get; set; }
        }

        public class LineBasedComparisonResultInternal
        {
            public ComparisonResultType ComparisonType { get; set; }
            public TextLines[] Text1 { get; set; }
            public TextLines[] Text2 { get; set; }
        }

        public class TextLines
        {
            public int LineNumber { get; set; }
            public String Text { get; set; }
        }
        
        public class TextComparisonResult
        {
            public LineBasedComparisonResultInternal[] LineBasedComparisonResults { get; set; }
        }

        public enum SingleComparisonResultType
        {
            Equals,
            Different,
            Addition,
        }
    }
}