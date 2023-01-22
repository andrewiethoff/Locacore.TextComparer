using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest5
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "\n\n\nwxyz\nffstu",
                Text2 = "mnopqr\nggggg\nhhhh\ni\nwxyz\nijk",
                MinimumRange = 2,

                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Different, 1, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("mnopqr", ComparisonResultType.Addition, 1, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Different, 2, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("ggggg", ComparisonResultType.Addition, 2, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(3,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Different, 3, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(3,
                            new LineSegment[]
                            {
                                new LineSegment("hhhh", ComparisonResultType.Addition, 3, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText2 = new LineInformation(4,
                            new LineSegment[]
                            {
                                new LineSegment("i", ComparisonResultType.Addition, 4, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(4,
                            new LineSegment[]
                            {
                                new LineSegment("wxyz", ComparisonResultType.Equals, 4, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(5,
                            new LineSegment[]
                            {
                                new LineSegment("wxyz", ComparisonResultType.Equals, 5, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(5,
                            new LineSegment[]
                            {
                                new LineSegment("ffstu", ComparisonResultType.Different, 5, 1, 1),
                            }),
                        LineOfText2 = new LineInformation(6,
                            new LineSegment[]
                            {
                                new LineSegment("ijk", ComparisonResultType.Different, 6, 1, 1),
                            })
                    }
                }
            };

            result.FinalizeResult();

            return result;
        }
    }
}