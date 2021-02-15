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
            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
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
                var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);

                sut.SeedFirstBox();

                var solution = sut.PuzzleGrid;

                var box = solution.ByBox().First();

                AssertIfNotFullSet(box, PuzzleSize.NineByNine);
            }
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedSecondBox_Test1()
        {
            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
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
                var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
                sut.SeedFirstBox();

                sut.SeedSecondBox();

                var solution = sut.PuzzleGrid;

                var box = solution.ByBox().ToArray();

                AssertIfNotFullSet(box[0], PuzzleSize.NineByNine);
                AssertIfNotFullSet(box[1], PuzzleSize.NineByNine);
            }
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedSecondBox_SetBased_Test1()
        {
            HashSet<int>[] CreateBoxRowSet(Puzzle puzzleGrid)
            {
                return Enumerable.Range(0, puzzleGrid.Size.BoxSize())
                    .Select(i => new HashSet<int>())
                    .ToArray();
            }

            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
            sut.Random = new TestNotSoRandom();
            sut.SeedFirstBox();

            // to make code look like class method
            var PuzzleGrid = new Puzzle(PuzzleSize.NineByNine);
            var puzzleValues = new List<int>(Enumerable.Range(1, PuzzleGrid.Size.ToInt32()));

            // get box0 values by row
            var box0 = CreateBoxRowSet(PuzzleGrid);
            foreach (var row in Enumerable.Range(0, PuzzleGrid.Size.BoxSize()))
            {
                foreach (var col in Enumerable.Range(0, PuzzleGrid.Size.BoxSize()))
                {
                    box0[row].Add(sut.PuzzleGrid[new PuzzleCoordinate(row, col)] ?? -1);
                }
            }

            var box1 = CreateBoxRowSet(PuzzleGrid);
            var box1Row0Choices = Enumerable.Range(1, PuzzleGrid.Size.BoxSize() - 1)
                .SelectMany(i => box0[i])
                .OrderBy(x => sut.Random.GetRandomNumber())
                .Take(PuzzleGrid.Size.BoxSize());

            foreach (var value in box1Row0Choices)
            {
                box1[0].Add(value);
            }

            // TODO:  more to do!!!
            // TODO:  more to do!!!
            // TODO:  more to do!!!
            // TODO:  more to do!!!
            // TODO:  more to do!!!

            var solution = sut.PuzzleGrid;

            Approvals.Verify(solution);
        }

        [Fact]
        public void FasterPuzzleSolutionGenerator_SeedThirdBox_Test1()
        {
            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
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
                var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
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
            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);
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
                var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);

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
            var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);

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
                var sut = new OptimizedPuzzleSolutionGenerator(PuzzleSize.NineByNine);

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
