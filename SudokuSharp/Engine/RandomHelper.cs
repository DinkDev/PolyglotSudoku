namespace SudokuSharp.Engine
{
    using System;

    /// <summary>
    /// An interface based random number generator that allows test substitution!
    /// </summary>
    public class RandomHelper : IRandom
    {
        /// <summary>The random number generator.</summary>
        private static readonly Random RandomNumberGenerator = new Random();

        /// <summary>Gets a random non-negative integer strictly less than the specified maximum.</summary>
        /// <param name="max">The maximum integer that is strictly greater than the maximum value to be returned.</param>
        /// <returns>The random value.</returns>
        public int GetRandomNumber(int max = int.MaxValue)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), "Must be greater than 0");
            }

            var data = new byte[4];
            RandomNumberGenerator.NextBytes(data);
            return (int)(BitConverter.ToUInt32(data, 0) % max);
        }

        /// <summary>Gets a random non-negative integer strictly less than the specified maximum and greater than or equal to the specified minimum.</summary>
        /// <param name="min">The minimum integer that is less than or equal to the value to be returned.</param>
        /// <param name="max">The maximum integer that is strictly greater than the maximum value to be returned.</param>
        public int GetRandomNumber(int min, int max)
        {
            var range = max - min;

            return range > 0 ? GetRandomNumber(range) + min : min;
        }
    }
}
