using System.Collections.Generic;

namespace Locacore.TextComparer
{
    public interface ILineBasedTextComparer
    {
        List<LineBasedComparisonResult> CompareTextsLineBased(string text1, string text2);
    }
}
