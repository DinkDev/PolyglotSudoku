namespace SudokuSharp.Engine.Solvers.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Xunit;
    using Xunit.Abstractions;

    [UseReporter(typeof(WinMergeReporter))]
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

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedFirstBox_Test1()
        {
            var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.Random = new TestNotSoRandom();
            
            sut.SeedFirstBox();

            var solution = sut.PuzzleGrid;

            Approvals.Verify(solution);
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedFirstBox_Test2()
        {
            for (var count = 0; count < 1000; count++)
            {
                var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);

                sut.SeedFirstBox();

                var solution = sut.PuzzleGrid;

                var box = solution.ByBox().First();

                AssertIfNotFullSet(box, PuzzleSize.NineByNine);
            }
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedSecondBox_Test1()
        {
            var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.Random = new TestNotSoRandom();
            sut.SeedFirstBox();

            sut.SeedSecondBox();

            var solution = sut.PuzzleGrid;

            Approvals.Verify(solution);
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedSecondBox_Test2()
        {
            for (var count = 0; count < 1000; count++)
            {
                var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
                sut.SeedFirstBox();

                sut.SeedSecondBox();

                var solution = sut.PuzzleGrid;

                var box = solution.ByBox().ToArray();

                AssertIfNotFullSet(box[0], PuzzleSize.NineByNine);
                AssertIfNotFullSet(box[1], PuzzleSize.NineByNine);
            }
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedThirdBox_Test1()
        {
            var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.Random = new TestNotSoRandom();
            sut.SeedFirstBox();
            sut.SeedSecondBox();

            sut.SeedThirdBox();

            var solution = sut.PuzzleGrid;

            Approvals.Verify(solution);
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedThirdBox_Test2()
        {
            for (var count = 0; count < 1000; count++)
            {
                var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
                sut.SeedFirstBox();
                sut.SeedSecondBox();

                sut.SeedThirdBox();

                var solution = sut.PuzzleGrid;

                var box = solution.ByBox().ToArray();

                AssertIfNotFullSet(box[0], PuzzleSize.NineByNine);
                AssertIfNotFullSet(box[1], PuzzleSize.NineByNine);
                AssertIfNotFullSet(box[2], PuzzleSize.NineByNine);
            }
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedFirstColumn_Test1()
        {
            var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.Random = new TestNotSoRandom();
            sut.SeedFirstBox();
            sut.SeedSecondBox();
            sut.SeedThirdBox();

            sut.SeedFirstColumn();

            var solution = sut.PuzzleGrid;

            Approvals.Verify(solution);
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedFirstColumn_Test2()
        {
            for (var count = 0; count < 1000; count++)
            {
                var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);

                sut.SeedFirstBox();
                sut.SeedSecondBox();
                sut.SeedThirdBox();
                sut.SeedFirstColumn();

                var solution = sut.PuzzleGrid;

                var column = solution.ByCol().First();

                AssertIfNotFullSet(column, PuzzleSize.NineByNine);
            }
        }






        [Fact]
        public void FasterPuzzleSolutionGenerator_CreatePuzzleSolution_Test1()
        {
            var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);

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

        [Fact]
        public void FasterPuzzleSolutionGenerator_CreatePuzzleSolution_Test2()
        {
            var t = new Stopwatch();
            t.Start();
            for (var count = 0; count < 100; count++)
            {
                var sut = new FasterPuzzleSolutionGenerator(PuzzleSize.NineByNine);

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
            t.Stop();

            TestHelper.WriteLine($"{100} puzzles created in {t.ToString()}");
        }

        private void AssertIfNotFullSet(IEnumerable<PuzzleCoordinateAndValue> sudokuSet, PuzzleSize puzzleSize)
        {
            var maxVal = puzzleSize.ToInt32();

            var sudokuArray = sudokuSet.ToArray();

            var setValues = sudokuArray.Select(s => s.Value)
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

    public class TestNotSoRandom : IRandom
    {
        public int NextValue { get; set; } = 0;
        public int Next()
        {
            return NextValue++;
        }

        public int GetRandomNumber(int count = int.MaxValue)
        {
            return Next() % count;
        }

        public int GetRandomNumber(int min, int max)
        {
            return (Next() + min) % max;
        }
    }
}
