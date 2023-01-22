using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Locacore.TextComparer;

namespace TextComparerUnitTests.ComparisonTests
{
    public class ComparisonTest9
    {
        private static readonly string LoremIpsum =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, ved riam nonumy eirmod tempor invidunt ut labore et tolaro magna aliquyam erat, mia separa voluptua.";

        private static string LoremIpsum100 = String.Join(" ", Enumerable.Range(1, 100).Select(x => LoremIpsum));
        
        public static TextComparerTestData TestData()
        {
            var result = new TextComparerTestData
            {
                Text1 = AddSpecificCharacter("abcdemnopqrstuvwxy aabbccddee " + LoremIpsum100, 'f'),
                Text2 = AddSpecificCharacter(LoremIpsum100, 'z'),
                MinimumRange = 2,
                ExpectedComparisonResult = new List<ComparisonResult>()
                {
                    new ComparisonResult(ComparisonResultType.Different,
                        AddSpecificCharacter("abcdemnopqrstuvwxy aabbccddee " + LoremIpsum100, 'f'),
                        AddSpecificCharacter(LoremIpsum100, 'z')),
                }
            };
            
            return result;
        }
        
        private static string AddSpecificCharacter(string text, char separatorChar)
        {
            Debug.Assert(!text.Contains(separatorChar));
            return string.Join(separatorChar, text.ToCharArray());
        }
    }
}