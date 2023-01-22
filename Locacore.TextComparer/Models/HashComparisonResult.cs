namespace Locacore.TextComparer
{
    internal class HashComparisonResult
    {
        public MatchInformation Position { get; private set; }
        public ComparisonResult Result { get; private set; }

        public HashComparisonResult(MatchInformation position, ComparisonResult result)
        {
            this.Position = position;
            this.Result = result;
        }
    }
}