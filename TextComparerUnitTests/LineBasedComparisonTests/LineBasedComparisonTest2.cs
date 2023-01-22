using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest2
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "abcde\r\ntuvwxyz",
                Text2 = "abcdet\nuvwxyz",
                MinimumRange = 1,
                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("abcde", ComparisonResultType.Equals, 1, 1, 1),
                                new LineSegment("", ComparisonResultType.Different, 1, 1, 1)
                            }),
                        LineOfText2 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("abcde", ComparisonResultType.Equals, 1, 1, 1),
                                new LineSegment("t", ComparisonResultType.Different, 1, 1, 1)
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("t", ComparisonResultType.Different, 2, 1, 1),
                                new LineSegment("uvwxyz", ComparisonResultType.Equals, 2, 1, 1)
                            }),
                        LineOfText2 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Different, 2, 1, 1),
                                new LineSegment("uvwxyz", ComparisonResultType.Equals, 2, 1, 1)
                            })
                    }
                }
            };

            result.FinalizeResult();

            return result;
        }
    }
}