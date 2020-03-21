namespace SudokuSharp.Engine
{
    /// <summary>
    /// The status of the Puzzle.
    /// </summary>
    public enum PuzzleStatus
    {
        Undefined,
        Solved,
        InProgress,
        Invalid,
        MultipleSolutions
    }
}