namespace SudokuSharp.Engine.Solvers.Tests
{
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

            foreach (var box in result.ByBox())
            {
                AssertIfNotFullSet(box, PuzzleSize.NineByNine);
            }

            foreach (var row in result.ByRow())
            {
                AssertIfNotFullSet(row, PuzzleSize.NineByNine);
            }

            foreach (var col in result.ByCol())
            {
                AssertIfNotFullSet(col, PuzzleSize.NineByNine);
            }
        }

        private void AssertIfNotFullSet(IEnumerable<PuzzleCoordinateAndValue> sudokuSet, PuzzleSize puzzleSize)
        {
            var maxVal = puzzleSize.ToInt32();

            var setValues = sudokuSet.Select(s => s.Value)
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .OrderBy((v => v))
                .Distinct()
                .ToList();

            Assert.Equal(maxVal, setValues.Count);
            Assert.Equal(1, setValues.First());
            Assert.Equal(maxVal, setValues.Last());
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
        public void PuzzleSolutionGenerator_RecursiveFillPuzzleBox_Test2()
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
}
