namespace Locacore.TextComparer
{
    public class TextComparerConfiguration
    {
        public int MinimumSizeForRangesOfDifferentText { get; set; }

        public TextComparerConfiguration()
        {
            this.MinimumSizeForRangesOfDifferentText = 5;
        }
    }
}