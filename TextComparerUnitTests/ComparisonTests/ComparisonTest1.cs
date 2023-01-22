using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest1
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "abcde",
                Text2 = "cdefg",
                MinimumRange = 1,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Deletion, "ab", ""),
                    new ComparisonResult(ComparisonResultType.Equals, "cde", "cde"),
                    new ComparisonResult(ComparisonResultType.Addition, "", "fg")
                }
            };
            
            return result;
        }
    }
}