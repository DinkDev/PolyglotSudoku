namespace SudokuSharp.Engine.Solvers
{
    public interface IPuzzleGeneratorOptions
    {
        int GetMaximumFilledCells(PuzzleSize puzzleSize);
        int GetMinimumFilledCells(PuzzleSize puzzleSize);
        int SeedCount { get; }
    }
}