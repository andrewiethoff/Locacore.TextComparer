using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest5
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "abcdef",
                Text2 = "",
                MinimumRange = 3,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Deletion, "abcdef", ""),
                }
            };
            
            return result;
        }
    }
}