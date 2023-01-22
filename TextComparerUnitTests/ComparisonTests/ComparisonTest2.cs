using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest2
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "abcdmhi",
                Text2 = "abedghi",
                MinimumRange = 1,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Equals, "ab", "ab"),
                    new ComparisonResult(ComparisonResultType.Different, "c", "e"),
                    new ComparisonResult(ComparisonResultType.Equals, "d", "d"),
                    new ComparisonResult(ComparisonResultType.Different, "m", "g"),
                    new ComparisonResult(ComparisonResultType.Equals, "hi", "hi"),
                }
            };
            
            return result;
        }
    }
}