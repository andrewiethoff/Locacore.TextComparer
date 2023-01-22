using System;
using System.Collections.Generic;
using System.Linq;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest8
    {
        private static readonly string LoremIpsum =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, ved riam nonumy eirmod tempor invidunt ut labore et tolaro magna aliquyam erat, mia separa voluptua.";

        private static string LoremIpsum100 = String.Join(" ", Enumerable.Range(1, 100).Select(x => LoremIpsum));
        
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = LoremIpsum100 + "abcd" + LoremIpsum100.Substring(0, LoremIpsum100.Length / 2 - 50) +
                        LoremIpsum100.Substring(LoremIpsum100.Length / 2),
                Text2 = LoremIpsum100 + LoremIpsum100,
                MinimumRange = 3,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Equals, LoremIpsum100, LoremIpsum100),
                    new ComparisonResult(ComparisonResultType.Deletion, "abcd", ""),
                    new ComparisonResult(ComparisonResultType.Equals,
                        LoremIpsum100.Substring(0, LoremIpsum100.Length / 2 - 50),
                        LoremIpsum100.Substring(0, LoremIpsum100.Length / 2 - 50)),
                    new ComparisonResult(ComparisonResultType.Addition, "",
                        LoremIpsum100.Substring(LoremIpsum100.Length / 2 - 50, 50)),
                    new ComparisonResult(ComparisonResultType.Equals,
                        LoremIpsum100.Substring(LoremIpsum100.Length / 2),
                        LoremIpsum100.Substring(LoremIpsum100.Length / 2)),
                }
            };
            
            return result;
        }
    }
}