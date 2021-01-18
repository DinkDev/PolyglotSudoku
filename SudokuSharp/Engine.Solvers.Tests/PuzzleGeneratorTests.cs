namespace SudokuSharp.Engine.Solvers.Tests
{
    using Generators;
    using Xunit;
    using Xunit.Abstractions;

    public class PuzzleGeneratorTests
    {
        public ITestOutputHelper TestHelper { get; set; }

        public PuzzleGeneratorTests(ITestOutputHelper testHelper)
        {
            TestHelper = testHelper;
        }

        [Fact]
        public void PuzzleGenerator_Test1()
        {
            var puzzleSolver = new DlxPuzzleSolver();
            var generatorOptions = new TestPuzzleGeneratorOptions();
            var clueGenerator = new SymmetricClueCoordinateGenerator();
            var puzzleSolution = (new PuzzleSolutionGenerator(PuzzleSize.NineByNine)).CreatePuzzleSolution();

            var sut = new PuzzleGenerator(generatorOptions, puzzleSolver, clueGenerator, puzzleSolution);

            var result = sut.Generate();

            Assert.NotNull(result);

            TestHelper.WriteLine(result.AsText());
        }

        [Fact]
        public void PuzzleGenerator_Test2()
        {
            for (var count = 0; count < 10; count++)
            {
                var puzzleSolver = new DlxPuzzleSolver();
                var generatorOptions = new TestPuzzleGeneratorOptions();
                var clueGenerator = new SymmetricClueCoordinateGenerator();
                var puzzleSolution = (new PuzzleSolutionGenerator(PuzzleSize.NineByNine)).CreatePuzzleSolution();

                var sut = new PuzzleGenerator(generatorOptions, puzzleSolver, clueGenerator, puzzleSolution);

                var result = sut.Generate();

                Assert.NotNull(result);

                TestHelper.WriteLine($"Puzzle {count}: {result.AsText()}");
            }
        }
    }
}