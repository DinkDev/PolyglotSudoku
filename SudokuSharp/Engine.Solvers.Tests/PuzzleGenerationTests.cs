namespace SudokuSharp.Engine.Solvers.Tests
{
    using Xunit;
    using Xunit.Abstractions;

    public class PuzzleGenerationTests
    {
        public ITestOutputHelper TestHelper { get; set; }

        public PuzzleGenerationTests(ITestOutputHelper testHelper)
        {
            TestHelper = testHelper;
        }

        [Fact]
        public void LetsTryToGenerateAPuzzle_Test1()
        {
            var puzzleGrid = new Puzzle(PuzzleSize.NineByNine);


            //puzzleGrid.LoadPuzzle(new string('.', 81));
            //AddRandomGivens(puzzleGrid);

            //var sut = new DlxPuzzleSolver();

            //sut.Solve(puzzleGrid, 50);

            //Assert.Equal(50, sut.SolutionList.Count);
        }
    }
}