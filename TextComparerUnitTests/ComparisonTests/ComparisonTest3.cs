using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest3
    {
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = "qwabcdmhias",
                Text2 = "qwabedghias",
                MinimumRange = 3,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Equals, "qwab", "qwab"),
                    new ComparisonResult(ComparisonResultType.Different, "cdm", "edg"),
                    new ComparisonResult(ComparisonResultType.Equals, "hias", "hias"),
                }
            };
            
            return result;
        }
    }
}