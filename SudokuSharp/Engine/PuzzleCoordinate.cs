namespace SudokuSharp.Engine
{
    using System;
    using System.Diagnostics;

    // TODO: would be nice to make PuzzleCoordinate not newable and have methods return a predefined set for each PuzzleSize!

    /// <summary>
    /// Struct for sudoku puzzle coordinates (zero based).
    /// </summary>
    [DebuggerDisplay("R{Row}, C{Col}")]
    public struct PuzzleCoordinate : IComparable<PuzzleCoordinate>
    {
        public int Col { get; }
        public int Row { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public PuzzleCoordinate(int row, int col)
        {
            Row = row;
            Col = col;
        }

        /// <summary>
        /// IComparable - for sorting!
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PuzzleCoordinate other)
        {
            var rv = Row - other.Row;
            if (rv == 0)
            {
                rv = Col - other.Col;
            }

            return rv;
        }
    }
}