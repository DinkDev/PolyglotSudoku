namespace SudokuSharp.Engine.Solvers.Tests
{
    using System.Linq;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Xunit;
    using Xunit.Abstractions;

    [UseReporter(typeof(BeyondCompareReporter))]
    public class DlxPuzzleSolverTests
    {
        public ITestOutputHelper TestHelper { get; set; }

        public DlxPuzzleSolverTests(ITestOutputHelper testHelper)
        {
            TestHelper = testHelper;
        }

        [Fact]
        public void DlxPuzzleSolver_SingleSolutions_Test1()
        {
            var puzzleText = new[]
            {
                ".4..756..",
                "...1.3594",
                "...64.2..",
                "13.....62",
                "48.....39",
                "26.....57",
                "..1.98...",
                "8943.6...",
                "..671..8."
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();

            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        [Fact]
        public void DlxPuzzleSolver_MultipleSolutions_Test1()
        {
            var puzzleText = new[]
            {
                "9.6.7.4.3",
                "...4..2..",
                ".7..23.1.",
                "5.....1..",
                ".4.2.8.6.",
                "..3.....5",
                ".3.7...5.",
                "..7..5...",
                "4.5.1.7.8"
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        /// <summary>
        /// http://www.sudokuprofessor.com/watch-where-you-get-those-puzzles-part-1-of-2/
        /// </summary>
        [Fact]
        public void DlxPuzzleSolver_MultipleSolutions_Test2()
        {
            var puzzleText = new[]
            {
                "..8.6..7.",
                "...4..693",
                "4.3.5....",
                "...5.426.",
                "294.76...",
                "......9.4",
                "....15...",
                ".59...746",
                ".826...1."
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        /// <summary>
        /// Weekly "unsolvable" #164 from SudokuWiki
        /// </summary>
        [Fact]
        public void DlxPuzzleSolver_CanSolveUnsolvable_Test1()
        {
            var puzzleText = new[]
            {
                "1.....7..",
                ".5...91..",
                ".98....5.",
                "...3.....",
                ".4..2.3..",
                "8....1.9.",
                "..26.....",
                "....4...6",
                "9....5.7."
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        /// <summary>
        /// Weekly "unsolvable" #155 from SudokuWiki
        /// </summary>
        [Fact]
        public void DlxPuzzleSolver_CanSolveUnsolvable_Test2()
        {
            var puzzleText = new[]
            {
                ".6.7.....",
                "7...5...3",
                "..5..21..",
                ".3.....4.",
                "4...6....",
                "..1..59..",
                "..8......",
                ".....8.59",
                "...12.8.."
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        /// <summary>
        /// Also from http://www.sudokuprofessor.com/watch-where-you-get-those-puzzles-part-1-of-2/
        /// </summary>
        [Fact]
        public void DlxPuzzleSolver_CanSolveUnsolvable_Test3()
        {
            var puzzleText = new[]
            {
                "..7....1.",
                "..1.7....",
                ".6..2174.",
                "2.......1",
                "65.3...72",
                "8..75....",
                "1.6.374..",
                "....48.9.",
                ".8..9...7"
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Approvals.VerifyAll(sut.SolutionList, string.Empty);
        }

        [Fact]
        public void DlxPuzzleSolver_CannotSolvePuzzleWithError_Test1()
        {
            var puzzleText = new[]
            {
                "9.6.7.413",
                "...4..2..",
                ".7..23.1.",
                "5.....1..",
                ".4.2.8.6.",
                "..3.....5",
                ".3.7...5.",
                "..7..5...",
                "4.5.1.7.8"
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Assert.False(sut.SolutionList.Any());
        }

        [Fact]
        public void DlxPuzzleSolver_CannotSolvePuzzleWithError_Test2()
        {
            var puzzleText = new[]
            {
                "123......",
                "456......",
                "789......",
                ".........",
                ".........",
                ".........",
                "745......",
                "823......",
                "61......."     // must be 619, but cannot be 619!
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid);

            Assert.False(sut.SolutionList.Any());
        }

        [Fact]
        public void DlxPuzzleSolver_CanSolvePuzzleNTimes_Test1()
        {
            var puzzleText = new[]
            {
                "123456789",
                ".........",
                ".........",
                ".........",
                ".........",
                ".........",
                ".........",
                ".........",
                "........."
            };

            var puzzleGrid = new Puzzle();
            puzzleGrid.LoadPuzzle(puzzleText);

            var sut = new DlxPuzzleSolver();
            puzzleGrid.LoadPuzzle(puzzleText);

            sut.Solve(puzzleGrid, 100);

            Assert.Equal(100, sut.SolutionList.Count);
        }

        [Fact]
        public void DlxPuzzleSolver_CanSolvePuzzleNTimes_Test2()
        {
            var puzzleGrid = new Puzzle(PuzzleSize.NineByNine);
            puzzleGrid.LoadPuzzle(new string('.', 81));
            AddRandomGivens(puzzleGrid);

            var sut = new DlxPuzzleSolver();

            sut.Solve(puzzleGrid, 50);

            Assert.Equal(50, sut.SolutionList.Count);
        }

        private void AddRandomGivens(Puzzle puzzle)
        {
            var random = new RandomHelper();
            var puzzleSize = puzzle.Size.ToInt32();

            for (byte n = 1; n <= puzzleSize; n++)
            {
                PuzzleCoordinate coordinate;
                do
                {
                    var x = random.GetRandomNumber(1, puzzleSize);
                    var y = random.GetRandomNumber(1, puzzleSize);
                    coordinate = new PuzzleCoordinate(x, y);
                }
                while (puzzle[coordinate].HasValue);
                puzzle[coordinate] = n;
            }
        }
    }
}
