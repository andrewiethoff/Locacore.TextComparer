using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest7
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "",
                Text2 = "",
                MinimumRange = 3,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                }
            };
            
            return result;
        }
    }
}