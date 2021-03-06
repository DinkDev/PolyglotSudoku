﻿namespace SudokuSharp.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods to give some nice functionality to use with the PuzzleSize enum.
    /// </summary>
    public static class PuzzleSizeExtensions
    {
        /// <summary>
        /// How many rows/columns in a box?
        /// </summary>
        /// <param name="puzzleSize">The size opf the overall puzzle</param>
        /// <returns>The square root of the PuzzleSize</returns>
        public static int BoxSize(this PuzzleSize puzzleSize)
        {
            if (puzzleSize == PuzzleSize.Undefined)
            {
                throw new ArgumentOutOfRangeException(nameof(puzzleSize), "Must be defined");
            }

            var boxSize = (int)Math.Sqrt(puzzleSize.ToInt32());

            return boxSize;
        }

        /// <summary>
        /// Get's the center coordinate of a puzzle, if PuzzleSize.IsOdd() is true!
        /// </summary>
        /// <param name="puzzleSize">The PuzzleSize of the puzzle.</param>
        /// <returns>The center coordinate of the puzzle, if it has one.</returns>
        public static PuzzleCoordinate GetCenterCoordinate(this PuzzleSize puzzleSize)
        {
            if (puzzleSize != PuzzleSize.NineByNine)
            {
                throw new ArgumentOutOfRangeException(nameof(puzzleSize), "Must be PuzzleSize.NineByNine");
            }

            var center = new PuzzleCoordinate(4, 4);

            return center;
        }

        /// <summary>
        /// Is the puzzle and odd number of rows, columns or boxes?
        /// </summary>
        /// <param name="puzzleSize">The PuzzleSize being checked.</param>
        /// <returns>true if odd, false if not</returns>
        public static bool IsOdd(this PuzzleSize puzzleSize)
        {
            if (puzzleSize == PuzzleSize.Undefined)
            {
                throw new ArgumentOutOfRangeException(nameof(puzzleSize),
                    "PuzzleSize.Undefined is neither even or odd");
            }

            return (puzzleSize == PuzzleSize.NineByNine);
        }

        /// <summary>
        /// Gets the integer value for the number of rows, columns or boxes defined by PuzzleSize.
        /// </summary>
        /// <param name="puzzleSize">The PuzzleSize getting the in value for.</param>
        /// <returns>The PuzzleSize's integer value</returns>
        public static int ToInt32(this PuzzleSize puzzleSize)
        {
            if (puzzleSize == PuzzleSize.Undefined)
            {
                throw new ArgumentOutOfRangeException(nameof(puzzleSize), "Must be defined");
            }

            return (int) puzzleSize;
        }

        public static IEnumerable<byte> ValidCellValues(this PuzzleSize puzzleSize)
        {
            return Enumerable.Range(1, puzzleSize.ToInt32()).Select(Convert.ToByte);
        }

        public static PuzzleSize[] ValidPuzzleSizes
        {
            get
            {
                return Enum.GetValues(typeof(PuzzleSize))
                    .Cast<PuzzleSize>()
                    .Where(v => v != PuzzleSize.Undefined)
                    .ToArray();
            }
        }

        public static PuzzleSize ToPuzzleSize(int size)
        {
            if (ValidPuzzleSizes.Any(v => v.ToInt32() == size))
            {
                return (PuzzleSize) size;
            }

            return PuzzleSize.Undefined;
        }
    }
}