namespace SudokuSharp.Engine.Solvers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class PuzzleSolutionGeneratorTests
    {
        public ITestOutputHelper TestHelper { get; set; }

        public PuzzleSolutionGeneratorTests(ITestOutputHelper testHelper)
        {
            TestHelper = testHelper;
        }

        [Fact]
        public void PuzzleSolutionGenerator_CreatePuzzleSolution_Test1()
        {
            var sut = new PuzzleSolutionGenerator(PuzzleSize.NineByNine);

            var result = sut.CreatePuzzleSolution();

            Assert.NotNull(result);
        }

        [Fact]
        public void PuzzleSolutionGenerator_RecursiveFillPuzzleBox_Test1()
        {
            var sut = new PuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.SeedPuzzleBox();

            var result = sut.RecursiveFillPuzzleBox(1, 0);

            Assert.True(result);

            var values = sut.PuzzleBoxes[1]
                .Select(p => p.Value)
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .Distinct()
                .OrderBy(v => v).ToList();

            Assert.Equal(PuzzleSize.NineByNine.ToInt32(), values.Count);
        }

        [Fact]
        public void PuzzleSolutionGenerator_RandomChoices_Test1()
        {
            var sut = new PuzzleSolutionGenerator(PuzzleSize.NineByNine);

            var result = sut.RandomChoices();

            var distinct = result.Distinct().ToList();

            Assert.Equal(PuzzleSize.NineByNine.ToInt32(), distinct.Count);
        }
    }

    public class PuzzleSolutionGenerator
    {
        public PuzzleSolutionGenerator(PuzzleSize puzzleSize)
        {
            PuzzleGrid = new Puzzle(puzzleSize);

            PuzzleBoxes = PuzzleGrid.ByBox()
                .Select(box => box.ToList()).ToList();
        }


        public List<List<PuzzleCoordinateAndValue>> PuzzleBoxes { get; set; }
        public Puzzle PuzzleGrid { get; set; }

        public Puzzle CreatePuzzleSolution()
        {
            SeedPuzzleBox();

            // More to do


            return PuzzleGrid;
        }

        /// <summary>
        /// Seed a box - this is guaranteed to be OK.
        /// </summary>
        /// <param name="box"></param>
        public void SeedPuzzleBox(int box = 0)
        {
            var choices = RandomChoices();
            foreach (var boxCell in Enumerable.Range(box, PuzzleGrid.Size.ToInt32()))
            {
                PuzzleBoxes[box][boxCell].Value = choices[boxCell];
            }
        }

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

        public List<byte> RandomChoices()
        {
            var maxValue = PuzzleGrid.Size.ToInt32();

            return Enumerable.Range(1, maxValue)
                .Select(Convert.ToByte)
                .OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
