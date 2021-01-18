namespace SudokuSharp.Engine.Solvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class for generating puzzles.
    /// </summary>
    public class PuzzleGenerator
    {
        private readonly IPuzzleGeneratorOptions _puzzleGeneratorOptions;
        private readonly IPuzzleSolver _puzzleSolver;
        private readonly IClueCoordinateGenerator _coordinateGenerator;

        /// <summary>
        /// Base ctor for PuzzleGenerator
        /// </summary>
        /// <param name="puzzleGeneratorOptions"></param>
        /// <param name="puzzleSolver"></param>
        /// <param name="coordinateGenerator"></param>
        /// <param name="puzzleSolution">The solved puzzle to generate a playable puzzle from.</param>
        public PuzzleGenerator(IPuzzleGeneratorOptions puzzleGeneratorOptions,
            IPuzzleSolver puzzleSolver,
            IClueCoordinateGenerator coordinateGenerator,
            Puzzle puzzleSolution)
        {
            _puzzleGeneratorOptions = puzzleGeneratorOptions ?? throw new ArgumentNullException(nameof(puzzleGeneratorOptions));
            _puzzleSolver = puzzleSolver ?? throw new ArgumentNullException(nameof(puzzleSolver));
            _coordinateGenerator = coordinateGenerator ?? throw new ArgumentNullException(nameof(coordinateGenerator));
            PuzzleSolution = puzzleSolution ?? throw new ArgumentNullException(nameof(puzzleSolution));
        }

        //public Stack<PuzzleCoordinateAndValue[]> PuzzleCluesCoordinates { get; set; }
        public Puzzle PuzzleToSolve { get; private set; }
        public Puzzle PuzzleSolution { get; }

        /// <summary>
        /// Main method to generate a new puzzle.
        /// </summary>
        /// <returns>true if successful, false otherwise.</returns>
        public Puzzle Generate()
        {
            var puzzleCluePairs =
                _coordinateGenerator.GenerateRandomClueCoordinates(PuzzleSolution).ToList();

            // ONE --->   seed
            var seededPuzzle = SeedPuzzleToSolve(puzzleCluePairs, _puzzleGeneratorOptions.SeedCount);
            PuzzleToSolve = seededPuzzle.Clone();

            // set any initial state

            // loop until we get a valid solution
            while (RecursiveGenerate(puzzleCluePairs) != NextStepToSolve.Solved)
            {
                // reset coordinates
                puzzleCluePairs =
                    _coordinateGenerator.GenerateRandomClueCoordinates(PuzzleSolution).ToList();

                // Try again with seeded puzzle
                PuzzleToSolve = seededPuzzle.Clone();
            }

            return PuzzleToSolve;
        }

        public Puzzle SeedPuzzleToSolve(List<PuzzleCoordinateAndValue[]> puzzleCluesCoordinates, int seedSetCount)
        {
            var rv = new Puzzle(PuzzleSolution.Size);

            while (seedSetCount-- > 0)
            {
                foreach (var seedCoordinate in puzzleCluesCoordinates[0])
                {
                    rv[seedCoordinate.Coordinate] = seedCoordinate.Value;
                    puzzleCluesCoordinates.RemoveAt(0);
                }
            }

            return rv;
        }

        /// <summary>
        /// The actual generation workhorse.
        /// </summary>
        /// <param name="puzzleCluePairs"></param>
        /// <returns></returns>
        private NextStepToSolve RecursiveGenerate(List<PuzzleCoordinateAndValue[]> puzzleCluePairs)
        {
            // test where we are at
            var rv = NextStep();

            // is there more to try?
            if (rv == NextStepToSolve.TooFewClues)
            {
                // try each available choice, until 1 works
                for (var current = 0; current < puzzleCluePairs.Count; current++)
                {
                    // try the current clues
                    var currentClues = puzzleCluePairs[current];

                    foreach (var clue in currentClues)
                    {
                        PuzzleToSolve[clue.Coordinate] = clue.Value;
                    }
                    
                    //  pass in copy of the remaining clues to a recursive call
                    var remaining = puzzleCluePairs.ToList();
                    remaining.RemoveAt(current);  
                    rv = RecursiveGenerate(remaining);

                    // if successful, it is done!
                    if (rv == NextStepToSolve.Solved)
                    {
                        break;
                    }

                    // else, undo this last try and try another
                    foreach (var clue in currentClues)
                    {
                        PuzzleToSolve[clue.Coordinate] = null;
                    }
                }
            }

            return rv;
        }

        // TODO: Next is to add quality checks -> no more than x clues in box, row or column.
        // TODO: Next is to add quality checks -> no more than x clues in box, row or column.
        // TODO: Next is to add quality checks -> no more than x clues in box, row or column.
        // TODO: Next is to add quality checks -> no more than x clues in box, row or column.

        private NextStepToSolve NextStep()
        {
            NextStepToSolve rv;
            var cluesPlaced = PuzzleToSolve.AsText(' ').Count(c => c != ' ');

            // are we over the limit?
            if (cluesPlaced <= _puzzleGeneratorOptions.GetMaximumFilledCells(PuzzleToSolve.Size))
            {
                // do we have some to add before trying to solve
                if (cluesPlaced >= _puzzleGeneratorOptions.GetMinimumFilledCells(PuzzleToSolve.Size))
                {
                    var solutions = _puzzleSolver.Solve(PuzzleToSolve, 2);
                    var solutionCount = solutions.Count;
                    rv = solutionCount == 1 
                        ? NextStepToSolve.Solved
                        : NextStepToSolve.TooFewClues;
                }
                else
                {
                    rv = NextStepToSolve.TooFewClues;
                }
            }
            else
            {
                rv = NextStepToSolve.TooManyClues;
            }

            return rv;
        }

    }
}