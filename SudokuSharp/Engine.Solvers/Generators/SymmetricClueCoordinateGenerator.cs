namespace SudokuSharp.Engine.Solvers.Generators
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// IClueCoordinateGenerator for symmetric puzzle clues.
    /// </summary>
    public class SymmetricClueCoordinateGenerator : IClueCoordinateGenerator
    {
        /// <summary>
        /// This does do symmetric.
        /// </summary>
        public bool Symmetric => true;

        /// <summary>
        /// Random number generator (can be set for testing).
        /// </summary>
        public IRandom Random { get; set; } = new RandomHelper();

        /// <summary>
        /// Generates a list of MaximumClueCells clue coordinates.
        /// </summary>
        /// <param name="puzzleSolution">A solved puzzle to get the clues from</param>
        /// <returns></returns>
        public IEnumerable<PuzzleCoordinateAndValue[]> GenerateRandomClueCoordinates(Puzzle puzzleSolution)
        {
            var sourceList = new List<PuzzleCoordinateAndValue>();
            var puzzleSize = puzzleSolution.Size.ToInt32();

            for (var row = 0; row < puzzleSize; row++)
            {
                for (var col = 0; col < puzzleSize; col++)
                {
                    var coordinate = new PuzzleCoordinate(row, col);
                    var value = puzzleSolution[coordinate];

                    sourceList.Add(new PuzzleCoordinateAndValue(coordinate, value));
                }
            }

            // just truncate at half way and app the points and their symmetric counterparts
            var pairList = new List<PuzzleCoordinateAndValue[]>();
            foreach (var cell in sourceList.Take(sourceList.Count / 2).ToList())
            {
                var symRow = puzzleSize - cell.Coordinate.Row - 1;
                var symCol = puzzleSize - cell.Coordinate.Col - 1;
                var symCoordinate = new PuzzleCoordinate(symRow, symCol);
                var symValue = puzzleSolution[symCoordinate];


                var symCell = new PuzzleCoordinateAndValue(symCoordinate, symValue);
                pairList.Add(new[] { cell, symCell });
            }

            // Add center coordinate, if any, as a single
            if (puzzleSolution.Size.IsOdd())
            {
                var centerCoordinate = puzzleSolution.Size.GetCenterCoordinate();
                var centerValue = puzzleSolution[centerCoordinate];

                pairList.Add(new[] { new PuzzleCoordinateAndValue(centerCoordinate, centerValue) });
            }

            // return a stack of randomized pairs
            return new Stack<PuzzleCoordinateAndValue[]>(pairList
                .OrderBy(x => Random.GetRandomNumber()));
        }
    }
}