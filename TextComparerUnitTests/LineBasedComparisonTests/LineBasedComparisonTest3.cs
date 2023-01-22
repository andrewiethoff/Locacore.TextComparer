using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest3
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "abcde\r\n\r\ntuvwxyz",
                Text2 = "abcde\ntuvwxyz\nijk",
                MinimumRange = 1,

                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("abcde", ComparisonResultType.Equals, 1, 1, 1),

                                new LineSegment("", ComparisonResultType.Addition, 1, 1, 1),
                            }),

                        LineOfText2 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("abcde", ComparisonResultType.Equals, 1, 1, 1),

                                new LineSegment("", ComparisonResultType.Deletion, 1, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Addition, 2, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(3,
                            new LineSegment[]
                            {
                                new LineSegment("tuvwxyz", ComparisonResultType.Equals, 3, 1, 1),
                            }),

                        LineOfText2 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("tuvwxyz", ComparisonResultType.Equals, 2, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText2 = new LineInformation(3,
                            new LineSegment[]
                            {
                                new LineSegment("ijk", ComparisonResultType.Addition, 3, 1, 1),
                            })
                    }
                }
            };
            
            result.FinalizeResult();
            
            return result;
        }
    }
}