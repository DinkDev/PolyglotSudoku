namespace SudokuSharp.Engine.Solvers.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var puzzleBoxes = puzzleGrid.ByBox()
                .Select(box => box.ToList()).ToList();




            //puzzleGrid.LoadPuzzle(new string('.', 81));
            //AddRandomGivens(puzzleGrid);

            //var sut = new DlxPuzzleSolver();

            //sut.Solve(puzzleGrid, 50);

            //Assert.Equal(50, sut.SolutionList.Count);
        }
    }
}