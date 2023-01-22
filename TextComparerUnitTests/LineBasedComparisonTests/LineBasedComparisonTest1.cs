using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest1
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "abcde",
                Text2 = "cdefg",
                MinimumRange = 1,
                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("ab", ComparisonResultType.Addition, 1, 1, 1),

                                new LineSegment("cde", ComparisonResultType.Equals, 1, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Deletion, 1, 1, 1),

                                new LineSegment("cde", ComparisonResultType.Equals, 1, 1, 1),

                                new LineSegment("fg", ComparisonResultType.Addition, 1, 1, 1),
                            })
                    }
                }
            };

            result.FinalizeResult();

            return result;
        }
    }
}