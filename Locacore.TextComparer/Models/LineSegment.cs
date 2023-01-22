using System;

namespace Locacore.TextComparer
{
    public class LineSegment : KeyProvider, IComparable<LineSegment>
    {
        public LineSegment(string text, ComparisonResultType type, int lineNumber, int blockNumber, int blockLineNumber)
        {
            this.Text = text;
            this.ComparisonType = type;
            this.LineNumber = lineNumber;
            this.BlockNumber = blockNumber;
            this.BlockLineNumber = blockLineNumber;
        }

        public string Text { get; private set; }
        public ComparisonResultType ComparisonType { get; private set; }
        internal int LineNumber { get; private set; }
        internal int BlockNumber { get; private set; }
        internal int BlockLineNumber { get; private set; }

        public int CompareTo(LineSegment other)
        {
            if (other == null) return -1;

            if (this.BlockNumber < other.BlockNumber) return -1;
            if (this.BlockNumber > other.BlockNumber) return 1;
            if (this.BlockLineNumber < other.BlockLineNumber) return -1;
            if (this.BlockLineNumber > other.BlockLineNumber) return 1;

            return 0;
        }

        public static bool operator >(LineSegment operand1, LineSegment operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        public static bool operator <(LineSegment operand1, LineSegment operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        public static bool operator >=(LineSegment operand1, LineSegment operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        public static bool operator <=(LineSegment operand1, LineSegment operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
    }
}