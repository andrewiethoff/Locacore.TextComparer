using System.Collections.Generic;
using Locacore.TextComparer;

namespace TextComparerUnitTests
{
    public class TextComparerTestData
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public int MinimumRange { get; set; }
        public List<ComparisonResult> ExpectedComparisonResult { get; set; }
    }

    public class LineBasedTextComparerTestData
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public int MinimumRange { get; set; }
        public List<LineBasedComparisonResult> ExpectedLineBasedComparisonResult { get; set; }

        public void FinalizeResult()
        {
            foreach (var line in this.ExpectedLineBasedComparisonResult)
            {
                if (line.LineOfText2 == null)
                    line.LineOfText1?.ReevaluteAggregatedResultTypeWhenComparedValueIsNull();

                if (line.LineOfText1 == null)
                    line.LineOfText2?.ReevaluteAggregatedResultTypeWhenComparedValueIsNull();
            }
        }
    }
}