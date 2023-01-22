namespace Locacore.TextComparer
{
    internal class MatchInformation
    {
        // Positions of the given match within the two texts

        public int PositionInText1 { get; private set; }
        public int PositionInText2 { get; private set; }

        // The length of the match, at the beginning only the Fibonacci size
        // and after the merge probably larger

        public int Length { get; private set; }

        // The offset between the two matches (which is the difference
        // within the positions)

        public int Offset { get; private set; }

        public MatchInformation(int positionInText1, int positionInText2, int len)
        {
            this.PositionInText1 = positionInText1;
            this.PositionInText2 = positionInText2;
            this.Length = len;
            this.Offset = positionInText2 - positionInText1;
        }
    }
}