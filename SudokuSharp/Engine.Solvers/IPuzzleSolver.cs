namespace SudokuSharp.Engine.Solvers
{
    using System.Collections.Generic;

    public interface IPuzzleSolver
    {
        /// <summary>
        /// The entry point to the solver
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="maxSolutionToFind">The upper limit of the number of solutions to find, if any</param>
        IList<Puzzle> Solve(Puzzle puzzle, int? maxSolutionToFind = null);
    }
}