using System.Collections.Generic;

namespace Locacore.TextComparer
{
    internal class QueueItem
    {
        // The two texts to compare (or appropriate parts of them)

        public string Text1 { get; private set; }
        public string Text2 { get; private set; }

        // Positions of the two texts relative to the whole text

        public int RelativePosition1 { get; private set; }
        public int RelativePosition2 { get; private set; }

        // The results will be stored in the following list, which
        // is passed from iteration to iteration

        public List<HashComparisonResult> HashResults { get; private set; }

        public QueueItem(string text1, string text2, int relpos1, int relpos2, int hashLengthPosition, List<HashComparisonResult> hashResults)
        {
            this.Text1 = text1;
            this.Text2 = text2;
            this.RelativePosition1 = relpos1;
            this.RelativePosition2 = relpos2;
            this.HashResults = hashResults;
        }
    }
}