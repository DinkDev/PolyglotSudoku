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
        public void LetsTryToGenerateAPuzzle_Test1()
        {
            var sut = new PuzzleSolutionGenerator(PuzzleSize.NineByNine);

            var result = sut.CreatePuzzleSolution();

            Assert.NotNull(result);
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
            // seed first box - guaranteed to be ok
            var choices = RandomChoices();
            foreach (var boxCell in Enumerable.Range(0, PuzzleGrid.Size.ToInt32()))
            {
                PuzzleBoxes[0][boxCell].Value = choices[boxCell];
            }

            // More to do


            return PuzzleGrid;
        }

        public bool RecursiveFillPuzzle(int boxIndex, int cellIndex, List<byte> choices)
        {
            var choicesToUse = choices.ToList();

            if (cellIndex == 0 || choicesToUse.Count == 0)
            {
                choicesToUse = RandomChoices().ToList();
            }

            if (cellIndex < PuzzleGrid.Size.ToInt32())
            {

            }
            else
            {
                boxIndex = boxIndex + 1;
                cellIndex = 0;
                if (boxIndex < PuzzleGrid.Size.ToInt32())
                {

                }
            }


            return true;


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
