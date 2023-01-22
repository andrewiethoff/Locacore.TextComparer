using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests.LineBaseComparisonTests
{
    public class LineBasedComparisonTest7
    {
        public static LineBasedTextComparerTestData TestData()
        {
            var result = new LineBasedTextComparerTestData
            {
                Text1 = "",
                Text2 = "",
                MinimumRange = 2,

                ExpectedLineBasedComparisonResult = new List<LineBasedComparisonResult>()
                {
                }
            };
            
            result.FinalizeResult();

            return result;
        }
    }
}