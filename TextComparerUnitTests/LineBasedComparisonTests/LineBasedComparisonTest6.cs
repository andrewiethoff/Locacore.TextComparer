using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest6
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "wxyz\nffstu",
                Text2 = "",
                MinimumRange = 2,

                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("wxyz", ComparisonResultType.Addition, 1, 1, 1),
                            }),

                        LineOfText2 = new LineInformation(1,
                            new LineSegment[]
                            {
                                new LineSegment("", ComparisonResultType.Deletion, 1, 1, 1),
                            })
                    },
                    new LineBasedComparisonResult()
                    {
                        LineOfText1 = new LineInformation(2,
                            new LineSegment[]
                            {
                                new LineSegment("ffstu", ComparisonResultType.Addition, 2, 1, 1),
                            })
                    }
                }
            };

            result.FinalizeResult();

            return result;
        }
    }
}