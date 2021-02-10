namespace SudokuSharp.Engine
{
    using System;
    using System.Diagnostics;

    // TODO: would be nice to make PuzzleCoordinate not newable and have methods return a predefined set for each PuzzleSize!

    /// <summary>
    /// Struct for sudoku puzzle coordinates (zero based).
    /// </summary>
    [DebuggerDisplay("R{Row}, C{Col}")]
    public readonly struct PuzzleCoordinate : IComparable<PuzzleCoordinate>, IEquatable<PuzzleCoordinate>
    {
        public int Col { get; }
        public int Row { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="row">The row value, 0 based</param>
        /// <param name="col">The column value, 0 based</param>
        public PuzzleCoordinate(int row, int col)
        {
            Row = row;
            Col = col;
        }

        /// <inheritdoc/>
        public int CompareTo(PuzzleCoordinate other)
        {
            var rv = Row - other.Row;
            if (rv == 0)
            {
                rv = Col - other.Col;
            }

            return rv;
        }

        /// <inheritdoc/>
        public bool Equals(PuzzleCoordinate other)
        {
            return Col == other.Col && Row == other.Row;
        }

        public override bool Equals(object obj)
        {
            return obj is PuzzleCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Col, Row);
        }

        public override string ToString()
        {
            return $"{nameof(Row)}: {Row}, {nameof(Col)}: {Col}";
        }
    }
}