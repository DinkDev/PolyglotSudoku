namespace SudokuSharp.Engine.Solvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class that will randomly fill a puzzle grid with a correct solution.
    ///
    /// This is a precursor to a puzzle generator.
    /// </summary>
    public class PuzzleSolutionGenerator
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="puzzleSize">The size of the puzzle solution to generate</param>
        public PuzzleSolutionGenerator(PuzzleSize puzzleSize)
        {
            PuzzleGrid = new Puzzle(puzzleSize);

            PuzzleBoxes = PuzzleGrid.ByBox()
                .Select(box => box.ToList()).ToList();
        }

        /// <summary>
        /// A collection of the PuzzleGrid's cells by box.
        /// </summary>
        public List<List<PuzzleCoordinateAndValue>> PuzzleBoxes { get; set; }

        /// <summary>
        /// Random number generator (can be set for testing).
        /// </summary>
        public IRandom Random { get; set; } = new RandomHelper();

        /// <summary>
        /// The puzzle solution being constructed.
        /// </summary>
        public Puzzle PuzzleGrid { get; set; }

        /// <summary>
        /// Top level method to create a random puzzle solution.
        /// </summary>
        /// <returns>A random puzzle solution.</returns>
        public Puzzle CreatePuzzleSolution()
        {
            SeedPuzzleBox();

            while (!RecursiveFillPuzzle(1))
            {
                // loop until done
            }

            return PuzzleGrid;
        }

        /// <summary>
        /// Seed a box - this is guaranteed to be OK.
        /// </summary>
        /// <param name="box">The index of the box to seed.</param>
        public void SeedPuzzleBox(int box = 0)
        {
            var choices = RandomChoices();
            foreach (var boxCell in Enumerable.Range(0, PuzzleGrid.Size.ToInt32()))
            {
                PuzzleBoxes[box][boxCell].Value = choices[boxCell];
            }
        }

        /// <summary>
        /// Recursively fill the boxes
        /// </summary>
        /// <param name="boxIndex">The current box to fill</param>
        /// <returns>true if successful, false otherwise.</returns>
        public bool RecursiveFillPuzzle(int boxIndex)
        {
            var rv = true;

            if (boxIndex < PuzzleGrid.Size.ToInt32())
            {
                rv = RecursiveFillPuzzleBox(boxIndex, 0);
                if (rv)
                {
                    rv = RecursiveFillPuzzle(boxIndex + 1);

                    if (!rv)
                    {
                        ClearPuzzleBox(boxIndex);
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Clears all cells in a puzzle box.
        /// </summary>
        /// <param name="box"></param>
        public void ClearPuzzleBox(int box)
        {
            foreach (var boxCellIndex in Enumerable.Range(0, PuzzleGrid.Size.ToInt32()))
            {
                PuzzleBoxes[box][boxCellIndex].Value = null;
            }
        }

        /// <summary>
        /// Randomly fill a box (likely after box 0), checking sudoku rules along the way.
        /// The list of choices is randomly generated, each is tried until the recursion succeeds.
        /// If the list runs out, the method returns false.
        /// </summary>
        /// <param name="boxIndex">The box to fill</param>
        /// <param name="boxCellIndex">The current box cell to get a value for.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public bool RecursiveFillPuzzleBox(int boxIndex, int boxCellIndex)
        {
            var rv = true;

            if (boxCellIndex < PuzzleGrid.Size.ToInt32())
            {
                var boxCell = PuzzleBoxes[boxIndex][boxCellIndex];
                var usedChoices = UsedChoices(boxIndex, boxCell.Coordinate.Row, boxCell.Coordinate.Col);
                var choicesToUse = RandomChoices()
                    .Where(r => !usedChoices.Contains(r))
                    .ToList();

                rv = choicesToUse.Any();
                if (rv)
                {
                    foreach (var choice in choicesToUse)
                    {
                        boxCell.Value = choice;
                        rv = RecursiveFillPuzzleBox(boxIndex, boxCellIndex + 1);
                        if (rv)
                        {
                            break;
                        }
                    }

                    // clear current on failure
                    if (!rv)
                    {
                        boxCell.Value = null;
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// For a given cell, find all used values in the box, row and column.
        /// </summary>
        /// <param name="box">The box index</param>
        /// <param name="row">The row index</param>
        /// <param name="col">The columns index</param>
        /// <returns>AN ordered and distinct union of values in the three sets.</returns>
        public List<byte> UsedChoices(int box, int row, int col)
        {
            // start with box values
            var rv = (
                from cell in PuzzleBoxes[box]
                where cell.Value.HasValue
                select cell.Value.Value).ToList();

            var rowValues = (
                from colIndex in Enumerable.Range(0, PuzzleGrid.Size.ToInt32())
                select PuzzleGrid[new PuzzleCoordinate(row, colIndex)] into value
                where value.HasValue
                select value.Value).ToList();
            rv.AddRange(rowValues);

            var colValues = (
                from rowIndex in Enumerable.Range(0, PuzzleGrid.Size.ToInt32())
                select PuzzleGrid[new PuzzleCoordinate(rowIndex, col)] into value
                where value.HasValue
                select value.Value).ToList();
            rv.AddRange(colValues);

            return rv.OrderBy(v => v).Distinct().ToList();
        }

        /// <summary>
        /// Builds a randomly ordered list of all values from 1 to Puzzle size.
        /// </summary>
        /// <returns>The randomly ordered list of all values from 1 to Puzzle size.</returns>
        public List<byte> RandomChoices()
        {
            var maxValue = PuzzleGrid.Size.ToInt32();

            return Enumerable.Range(1, maxValue)
                .Select(Convert.ToByte)
                .OrderBy(x => Random.GetRandomNumber()).ToList();
        }
    }
}