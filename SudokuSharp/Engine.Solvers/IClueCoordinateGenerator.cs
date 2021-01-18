namespace SudokuSharp.Engine.Solvers
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for clue coordinate generators for puzzle generation
    /// </summary>
    public interface IClueCoordinateGenerator
    {
        bool Symmetric { get; }

        IEnumerable<PuzzleCoordinateAndValue[]> GenerateRandomClueCoordinates(Puzzle puzzleSolution);
    }
}