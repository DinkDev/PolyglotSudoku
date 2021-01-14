namespace SudokuSharp.Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class PuzzleExtensions
    {
        /// <summary>
        /// Gets a nested IEnumerable to traverse a Puzzle by sudoku boxes.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<PuzzleCoordinateAndValue>> ByBox(
            this Puzzle puzzle)
        {
            for (var box = 0; box < puzzle.Size.ToInt32(); box++)
            {
                yield return CellsByBox(puzzle, box);
            }
        }

        private static IEnumerable<PuzzleCoordinateAndValue> CellsByBox(
            Puzzle puzzle, int box)
        {
            var boxSize = puzzle.Size.BoxSize();
            var boxStartRow = (box / boxSize) * boxSize;
            var boxStartCol = (box % boxSize) * boxSize;

            for (var row = boxStartRow; row < boxStartRow + boxSize; row++)
            {
                for (var col = boxStartCol; col < boxStartCol + boxSize; col++)
                {
                    yield return puzzle[row, col];
                }
            }
        }

        /// <summary>
        /// Gets a nested IEnumerable to traverse a Puzzle by columns.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<PuzzleCoordinateAndValue>> ByCol(
            this Puzzle puzzle)
        {
            for (var col = 0; col < puzzle.Size.ToInt32(); col++)
            {
                yield return RowsByColumn(puzzle, col);
            }
        }

        private static IEnumerable<PuzzleCoordinateAndValue> RowsByColumn(
            Puzzle puzzle, int col)
        {
            for (var row = 0; row < puzzle.Size.ToInt32(); row++)
            {
                yield return puzzle[row, col];
            }
        }

        /// <summary>
        /// Gets a nested IEnumerable to traverse a Puzzle by rows.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<PuzzleCoordinateAndValue>> ByRow(
            this Puzzle puzzle)
        {
            for (var row = 0; row < puzzle.Size.ToInt32(); row++)
            {
                yield return ColumnsByRow(puzzle, row);
            }
        }

        private static IEnumerable<PuzzleCoordinateAndValue> ColumnsByRow(
            Puzzle puzzle, int row)
        {
            for (var col = 0; col < puzzle.Size.ToInt32(); col++)
            {
                yield return puzzle[row, col];
            }
        }

        /// <summary>
        /// Compares two puzzles.
        /// </summary>
        /// <param name="puzzle1">the first puzzle dictionary</param>
        /// <param name="puzzle2">the second puzzle dictionary</param>
        /// <returns>true if equal, false otherwise</returns>
        public static bool ComparePuzzle(this Puzzle puzzle1, Puzzle puzzle2)
        {
            if (puzzle1 == null)
            {
                throw new ArgumentNullException(nameof(puzzle1));
            }

            if (puzzle2 == null)
            {
                throw new ArgumentNullException(nameof(puzzle2));
            }

            var rv = false;

            if (puzzle1.Size == puzzle2.Size)
            {
                rv = !(from key in puzzle1.GetKeys()
                    let value1 = puzzle1[key]
                    let value2 = puzzle2[key]
                    where !value1.Equals(value2)
                    select value1).Any();
            }

            return rv;
        }

        /// <summary>
        /// Analyzes the puzzle's values to determine its current status.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <returns>The state of the puzzle.</returns>
        public static PuzzleStatus GetPuzzleStatus(this Puzzle puzzle)
        {
            if (puzzle == null)
            {
                throw new ArgumentNullException(nameof(puzzle));
            }

            // buffer for tracking used numbers in a ror, column or box
            var puzzleSize = puzzle.Size.ToInt32();
            var numbersUsed = new BitArray(puzzleSize);

            // Make sure every column contains the right numbers.  It's ok if a column has holes
            // as long as those cells have possibilities, in which case it's a puzzle in progress.
            // However, two numbers can't be used in the same column, even if there are holes.
            foreach (var row in puzzle.ByRow())
            {
                numbersUsed.SetAll(false);
                foreach (var cell in row)
                {
                    if (cell.Value.HasValue)
                    {
                        var value = cell.Value.Value;
                        if (numbersUsed.IsSet(value, 1))
                        {
                            return PuzzleStatus.Invalid;
                        }

                        numbersUsed.Set(value, 1, true);
                    }
                }
            }

            // Same for columns
            foreach (var col in puzzle.ByCol())
            {
                numbersUsed.SetAll(false);
                foreach (var cell in col)
                {
                    if (cell.Value.HasValue)
                    {
                        var value = cell.Value.Value;
                        if (numbersUsed.IsSet(value, 1))
                        {
                            return PuzzleStatus.Invalid;
                        }
                        numbersUsed.Set(value, 1, true);
                    }
                }
            }

            // Same for boxes
            foreach (var box in puzzle.ByBox())
            {
                numbersUsed.SetAll(false);
                foreach (var cell in box)
                {
                    if (cell.Value.HasValue)
                    {
                        var value = cell.Value.Value;
                        if (numbersUsed.IsSet(value, 1))
                        {
                            return PuzzleStatus.Invalid;
                        }
                        numbersUsed.Set(value, 1, true);
                    }
                }
            }

            // Now figure out if this is a solved puzzle or a work in progress
            // based on if there are any holes
            for (var row = 0; row < puzzleSize; row++)
            {
                for (var col = 0; col < puzzleSize; col++)
                {
                    var coord = new PuzzleCoordinate(row, col);
                    if (!puzzle[coord].HasValue)
                    {
                        return PuzzleStatus.InProgress;
                    }
                }
            }

            // If we made it this far, this state is a valid solution!  Woo hoo!
            return PuzzleStatus.Solved;
        }
    }
}