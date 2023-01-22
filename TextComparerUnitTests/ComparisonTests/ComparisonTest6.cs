using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest6
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "abcdef/uvwxyz|ghijklmnopq-rstab",
                Text2 = "abcdef|ghijklmnopq/uvwxyz-rstab",
                MinimumRange = 3,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Equals, "abcdef", "abcdef"),
                    new ComparisonResult(ComparisonResultType.Deletion, "/uvwxyz", ""),
                    new ComparisonResult(ComparisonResultType.Equals, "|ghijklmnopq", "|ghijklmnopq"),
                    new ComparisonResult(ComparisonResultType.Addition, "", "/uvwxyz"),
                    new ComparisonResult(ComparisonResultType.Equals, "-rstab", "-rstab"),
                }
            };
            
            return result;
        }
    }
}