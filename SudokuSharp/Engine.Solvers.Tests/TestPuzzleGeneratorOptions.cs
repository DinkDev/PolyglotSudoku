namespace SudokuSharp.Engine.Solvers.Tests
{
    public class TestPuzzleGeneratorOptions : IPuzzleGeneratorOptions
    {
        public int GetMaximumFilledCells(PuzzleSize puzzleSize)
        {
            return 29;
        }

        public int GetMinimumFilledCells(PuzzleSize puzzleSize)
        {
            return 17;
        }

        public int SeedCount { get; } = 0;
    }
}