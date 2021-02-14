namespace SudokuSharp.Engine.Solvers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class that will randomly fill a puzzle grid with a correct solution.
    ///
    /// It is optimized for quickly filling much of the grid, then recursively
    /// filling the rest.
    ///
    /// This is a precursor to a puzzle generator.
    /// </summary>
    /// <remarks>
    /// This was inspired by a paper by Daniel Beer.  His paper is posted at:
    /// https://dlbeer.co.nz/articles/sudoku.html
    /// </remarks>
    public class OptimizedPuzzleSolutionGenerator
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="puzzleSize">The size of the puzzle solution to generate</param>
        public OptimizedPuzzleSolutionGenerator(PuzzleSize puzzleSize)
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
            SeedFirstBox();

            if (PuzzleGrid.Size == PuzzleSize.NineByNine)
            {
                SeedSecondBox();
                SeedThirdBox();
            }

            SeedFirstColumn();

            var possibilities = InitializePossibilities();
            RecursiveFillPuzzle(possibilities);

            return PuzzleGrid;
        }

        /// <summary>
        /// Seed the first box - just pick each number from the set.
        /// 
        /// This is guaranteed to be OK (and quick).
        /// </summary>
        public void SeedFirstBox()
        {
            var choices = RandomChoices();
            foreach (var boxCell in Enumerable.Range(0, PuzzleGrid.Size.ToInt32()))
            {
                PuzzleBoxes[0][boxCell].Value = choices[boxCell];
            }
        }

        /// <summary>
        /// Seed the second box.
        ///
        /// This is more difficult, but still quick.
        /// 
        /// B2's 1st row can be any from B1's 2nd or 3rd
        /// B2's 2nd row must include any remaining from B2's 3rd row, fill in with B1's 1st row
        /// B2's 3rd row is all remaining
        /// </summary>
        public void SeedSecondBox()
        {
            var boxSize = PuzzleGrid.Size.BoxSize();
            var puzzleSize = PuzzleGrid.Size.ToInt32();

            var firstBoxUsed = CreateArray(boxSize, () => new BitArray(puzzleSize));

            // load first box used values
            foreach (var boxCell in Enumerable.Range(0, puzzleSize))
            {
                var cell = PuzzleBoxes[0][boxCell];
                var cellValue = cell.Value
                                ?? throw new Exception($"Cell at {cell.Coordinate} is null");

                firstBoxUsed[cell.Coordinate.Row][cellValue - 1] = true;
            }

            var usedByRow = CreateArray(boxSize, () => new BitArray(puzzleSize));

            // chose values for first row of second box (from first box, second and third rows)
            var firstRowPossibilities = new BitArray(firstBoxUsed[1]).Or(firstBoxUsed[2]);
            for (var i = 0; i < boxSize; i++)
            {
                var value = PickRandomValueFromBitArray(firstRowPossibilities);

                usedByRow[0][value - 1] = true;
                firstRowPossibilities[value - 1] = false;
            }

            // chose values for second row of the second box
            var secondRowPossibilities = (new BitArray(firstBoxUsed[0]).Or(firstBoxUsed[2])).And(new BitArray(usedByRow[0]).Not());
            var thirdRowPossibilities = (new BitArray(firstBoxUsed[0]).Or(firstBoxUsed[1])).And(new BitArray(usedByRow[0]).Not());

            while (thirdRowPossibilities.CountSetBits() > 3)
            {
                var value = PickRandomValueFromBitArray(secondRowPossibilities);

                usedByRow[1][value - 1] = true;
                secondRowPossibilities[value - 1] = false;
                thirdRowPossibilities[value - 1] = false;
            }

            // Value for bottom row
            usedByRow[1].Or(new BitArray(secondRowPossibilities).And(new BitArray(thirdRowPossibilities).Not()));
            usedByRow[2].Or(thirdRowPossibilities);

            // assign values to each row of the box
            for (var row = 0; row < boxSize; row++)
            {
                for (var col = boxSize; col < 2 * boxSize; col++)
                {
                    var value = PickRandomValueFromBitArray(usedByRow[row]);

                    PuzzleGrid[new PuzzleCoordinate(row, col)] = Convert.ToByte(value);
                    usedByRow[row][value - 1] = false;
                }
            }
        }

        public void SeedThirdBox()
        {
            var boxSize = PuzzleGrid.Size.BoxSize();
            var size = PuzzleGrid.Size.ToInt32();

            for (var row = 0; row < boxSize; row++)
            {
                var remainingValues = new BitArray(size, true);

                // Remove the values used in this row
                for (var col = 0; col < 2 * boxSize; col++)
                {
                    var coordinate = new PuzzleCoordinate(row, col);
                    var cellValue = PuzzleGrid[coordinate]
                                    ?? throw new Exception($"Cell at {coordinate} is null");

                    remainingValues[cellValue - 1] = false;
                }

                // Set the remaining values in the last boxes row
                for (var col = 2 * boxSize; col < size; col++)
                {
                    var value = PickRandomValueFromBitArray(remainingValues);

                    PuzzleGrid[new PuzzleCoordinate(row, col)] = Convert.ToByte(value);
                    remainingValues[value - 1] = false;
                }
            }
        }

        public void SeedFirstColumn()
        {
            var boxSize = PuzzleGrid.Size.BoxSize();
            var size = PuzzleGrid.Size.ToInt32();
            var col = 0;

            var remainingValues = new BitArray(size, true);

            int row;

            for (row = 0; row < boxSize; row++)
            {
                var cell = PuzzleGrid[new PuzzleCoordinate(row, col)];

                if (cell.HasValue)
                {
                    remainingValues[cell.Value - 1] = false;
                }
            }

            for (; row < size; row++)
            {
                var value = PickRandomValueFromBitArray(remainingValues);

                PuzzleGrid[new PuzzleCoordinate(row, col)] = Convert.ToByte(value);
                remainingValues[value - 1] = false;
            }
        }

        /// <summary>
        /// Recursively fill the rest of the puzzle
        /// </summary>
        /// <param name="possibilities">The current box to fill</param>
        /// <returns>true if successful, false otherwise.</returns>
        public bool RecursiveFillPuzzle(BitArray[,] possibilities)
        {
            var puzzleSize = PuzzleGrid.Size.ToInt32();

            var bestCell = FindCellWithFewestPossibilities(PuzzleGrid, possibilities);

            if (bestCell != null)
            {
                var bestPossibility = possibilities[bestCell.Value.Row, bestCell.Value.Col];

                while (bestPossibility.Cast<bool>().Any(t => t))
                {
                    var value = PickRandomValueFromBitArray(bestPossibility);

                    bestPossibility[value - 1] = false;
                    PuzzleGrid[bestCell.Value] = Convert.ToByte(value);

                    var newPossibilities = Clone2DArray(possibilities, x => new BitArray(x));

                    EliminatePossibilities(newPossibilities, bestCell.Value.Row, bestCell.Value.Col, value);

                    if (RecursiveFillPuzzle(newPossibilities))
                    {
                        return true;
                    }
                }

                PuzzleGrid[bestCell.Value] = null;
                return false;
            }

            return true;
        }

        private T[,] Clone2DArray<T>(T[,] orig, Func<T,T> newT)
        {
            var rows = orig.GetLength(0);
            var cols = orig.GetLength(1);
            var rv = new T[rows, cols];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    rv[row, col] = newT(orig[row, col]);
                }
            }

            return rv;
        }

        private T[] CreateArray<T>(int count, Func<T> newT)
        {
            return Enumerable.Range(0, count)
                .Select(c => newT())
                .ToArray();
        }

        public BitArray[,] InitializePossibilities()
        {
            var size = PuzzleGrid.Size.ToInt32();
            var rv = new BitArray[size, size];

            for (var row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    rv[row, col] = new BitArray(size, true);

                }
            }

            for (var row = 0; row < size; row++)
            {
                for (var col = 0; col < size; col++)
                {
                    var cell = PuzzleGrid[new PuzzleCoordinate(row,col)];

                    if (cell.HasValue)
                    {
                        EliminatePossibilities(rv, row, col, cell.Value);
                    }
                }
            }

            return rv;
        }

        public void EliminatePossibilities(BitArray[,] possibilities, int row, int col, int value)
        {
            var size = PuzzleGrid.Size.ToInt32();
            var mask = GetSetBit(value, negate: true);
            var saved = new BitArray(possibilities[row , col]);

            for (var currentRow = 0; currentRow < size; currentRow++)
            {
                possibilities[currentRow, col].And(mask);
            }

            for (var currentCol = 0; currentCol < size; currentCol++)
            {
                possibilities[row, currentCol].And(mask);
            }

            var boxSize = PuzzleGrid.Size.BoxSize();

            var rowStart = row - row % boxSize;
            var colStart = col - col % boxSize;
            for (var currentRow = rowStart; currentRow < rowStart + boxSize; currentRow++)
            {
                for (var currentCol = colStart; currentCol < colStart + boxSize; currentCol++)
                {
                    possibilities[currentRow, currentCol].And(mask);
                }
            }

            //possibilities[row, col] = saved;
            possibilities[row, col] = mask;
        }

        public BitArray GetSetBit(int value, bool negate = false)
        {
            var rv = new BitArray(PuzzleGrid.Size.ToInt32(), negate);
            if (value > 0)
            {
                rv[value - 1] = !negate;
            }
            return rv;
        }

        public PuzzleCoordinate? FindCellWithFewestPossibilities(Puzzle puzzle, BitArray[,] possibilities)
        {
            PuzzleCoordinate? bestIndex = null;
            var bestScore = -1;

            for (var row = 0; row < puzzle.Size.ToInt32(); row++)
            for (var col = 0; col < puzzle.Size.ToInt32(); col++)
            {
                var cellCoord = new PuzzleCoordinate(row, col);
                var cell = puzzle[cellCoord];

                if (!cell.HasValue)
                {
                    var score = possibilities[row, col].CountSetBits();

                    if (bestScore < 0 || score < bestScore)
                    {
                        bestIndex = cellCoord;
                        bestScore = score;
                    }
                }
            }

            return bestIndex;
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

        public int PickRandomValueFromBitArray(BitArray bitArray)
        {
            var rv = 0;
            var choices = Enumerable.Range(0, bitArray.Count)
                .Where(x => bitArray[x])
                .Select(x => x + 1)
                .ToArray();

            if (choices.Any())
            {
                var index = Random.GetRandomNumber() % choices.Length;
                rv = choices[index];
            }

            return rv;
        }
    }
}