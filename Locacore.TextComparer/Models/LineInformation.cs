using System.Linq;

namespace Locacore.TextComparer
{
    public class LineInformation : KeyProvider
    {
        public int LineNumber { get; private set; }
        public ComparisonResultType AggregatedResultType { get; private set; }
        public LineSegment[] LineTexts { get; private set; }

        public LineInformation(int lineNumber, LineSegment[] lineTexts)
        {
            this.LineNumber = lineNumber;
            this.LineTexts = lineTexts;

            // Check what the background status should be (different, addition or equal - 
            // there is no deleted in the line results anymore).

            // Difference has highest priority

            var isSegmentOfTypeDifferentAndNotEmpty = lineTexts.Any(x => (x.ComparisonType == ComparisonResultType.Different) && (!string.IsNullOrEmpty(x.Text)));
            if (isSegmentOfTypeDifferentAndNotEmpty)
            {
                this.AggregatedResultType = ComparisonResultType.Different;
            }
            else
            {
                // If not different, check for addition (or deletion, which is now handled
                // as addition). Otherwise the line has status "equal".

                var isSegmentOfTypeAdditionOrDeletionAndNotEmpty = lineTexts.Any(x => (x.ComparisonType == ComparisonResultType.Addition || x.ComparisonType == ComparisonResultType.Deletion) && (!string.IsNullOrEmpty(x.Text)));
                if (isSegmentOfTypeAdditionOrDeletionAndNotEmpty)
                {
                    this.AggregatedResultType = ComparisonResultType.Addition;
                }
                else
                {
                    this.AggregatedResultType = ComparisonResultType.Equals;
                }
            }
        }

        public void ReevaluteAggregatedResultTypeWhenComparedValueIsNull()
        {
            // Correct bad values, but only if the other text is null.
            // Therefore, text shouldn't be equal, but it is
            // in fact an addition of this text.

            if ((this.AggregatedResultType == ComparisonResultType.Equals))
            {
                this.AggregatedResultType = ComparisonResultType.Addition;
            }
        }

        internal bool SmallerThan(LineInformation other)
        {
            // If the other value is null, this one
            // is always smaller (by definition)

            if (other == null) return true;
            
            // "smaller than" (and thus Min/Max) is overwritten by the LineSegment class

            var otherMinimumLineBlockAndLineNumber = other.LineTexts.Where(x => ((!string.IsNullOrWhiteSpace(x.Text)) && (x.ComparisonType==ComparisonResultType.Equals))|| (other.LineTexts.Count() <= 1)).Min() ?? other.LineTexts.FirstOrDefault();
            var thisMaximumBlockAndLineNumber = this.LineTexts.Where(x => ((!string.IsNullOrWhiteSpace(x.Text)) && (x.ComparisonType==ComparisonResultType.Equals)) || (this.LineTexts.Count() <= 1)).Max() ?? this.LineTexts.FirstOrDefault();

            return thisMaximumBlockAndLineNumber < otherMinimumLineBlockAndLineNumber;
        }
    }
}