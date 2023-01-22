using System.Collections.Generic;

namespace Locacore.TextComparer
{
    public interface ITextComparer
    {
        List<ComparisonResult> CompareTexts(string text1, string text2);
    }
}