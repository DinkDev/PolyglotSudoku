namespace SudokuSharp.Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extensions for nice set functions on BitArrays.
    /// </summary>
    public static class BitArrayExtensions
    {
        /// <summary>
        /// Gets the number of bits set in the bitArray.
        /// </summary>
        /// <param name="bitArray">The BitArray to check</param>
        /// <returns>The number of bits set in the bitArray</returns>
        /// <remarks>
        /// This method is extremely fast - faster then Brian Kernighan's technique I left in below.
        /// See: http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        /// and http://stackoverflow.com/questions/5063178/counting-bits-set-in-a-net-bitarray-class
        /// </remarks>
        public static int CountSetBits(this BitArray bitArray)
        {
            var count = 0;
            var intArray = new int[(bitArray.Length >> 5) + 1];

            bitArray.CopyTo(intArray, 0);

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            // C# 8 is pretty cool (and more pythonic)!
            intArray[^1] &= ~(-1 << (bitArray.Length % 32));

            for (var i = 0; i < intArray.Length; i++)
            {
                var c = intArray[i];

                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c -= ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;
            }

            return count;
        }

        /// <summary>
        /// Gets the offset index of all bits set in the BitArray
        /// </summary>
        /// <param name="bitArray">BitArray to evaluate</param>
        /// <param name="offset">Offset to add to set bit indexes - if 1-9, then use 1, but if 0-8, use 0</param>
        /// <returns>An array of the indices of true values in the BitArray.</returns>
        public static int[] GetSetBits(this BitArray bitArray, int offset)
        {
            if (bitArray == null)
            {
                throw new ArgumentNullException(nameof(bitArray));
            }

            var setBits = bitArray.CountSetBits();
            var bits = new int[setBits];
            var j = 0;
            if (setBits > 0)
            {
                for (var i = 0; i < bitArray.Length; i++)
                {
                    if (bitArray[i])
                    {
                        bits[j++] = i + offset;
                    }
                }
            }

            return bits;
        }

        /// <summary>
        /// Gets the locations of all set bits in the BitArray
        /// </summary>
        /// <param name="bitArray">BitArray to evaluate</param>
        /// <returns>An array of the indices of true values in the BitArray.</returns>
        public static int[] GetSetBits(this BitArray bitArray)
        {
            return bitArray.GetSetBits(0);
        }

        /// <summary>Determines if the specified number is set in the bitArray.</summary>
        /// <param name="bitArray">The bit array.</param>
        /// <param name="number">The number to check for</param>
        /// <param name="offset">Offset to add to set bit indexes - if 1-9, then use 1, but if 0-8, use 0</param>
        /// <returns>true if ALL of the numbers in the bitArray are set, false if not</returns>
        public static bool IsSet(this BitArray bitArray, int number, int offset)
        {
            return bitArray[number - offset];
        }


        /// <summary>Determines if all of the specified numbers are set in the bitArray.</summary>
        /// <param name="bitArray">The bit array.</param>
        /// <param name="numbers">The numbers to check for</param>
        /// <param name="offset">Offset to add to set bit indexes - if 1-9, then use 1, but if 0-8, use 0</param>
        /// <returns>true if ALL of the numbers in the bitArray are set, false if not</returns>
        public static bool AllAreSet(this BitArray bitArray, IEnumerable<int> numbers, int offset)
        {
            return numbers.All(n => bitArray[n - offset]);
        }

        /// <summary>Determines if any of the specified numbers are set in the bitArray.</summary>
        /// <param name="bitArray">The bit array.</param>
        /// <param name="numbers">The numbers to check for</param>
        /// <param name="offset">Offset to add to set bit indexes - if 1-9, then use 1, but if 0-8, use 0</param>
        /// <returns>true if ANY of the numbers in the bit array are set, false if not</returns>
        public static bool AnyAreSet(this BitArray bitArray, IEnumerable<int> numbers, int offset)
        {
            return numbers.Any(n => bitArray[n - offset]);
        }

        /// <summary>Determines if two bitArrays have the same bitwise values.</summary>
        /// <param name="self">The left-hand side bit array.</param>
        /// <param name="other">The left-hand side bit array.</param>
        /// <returns>true if they both have the same bitwise values, false if not</returns>
        public static bool IsEqual(this BitArray self, BitArray other)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var rv = false;

            if (self.Length == other.Length)
            {
                var selfSetCount = self.CountSetBits();
                var otherSetCount = other.CountSetBits();
                if (selfSetCount == otherSetCount)
                {
                    // test with Xor
                    var selfMirror = new BitArray(self);
                    selfMirror.Xor(other);

                    // equal if all zeros!
                    rv = selfMirror.CountSetBits() == 0;
                }
            }

            return rv;
        }

        /// <summary>Assigns the specified number in the bitArray.</summary>
        /// <param name="bitArray">The bit array.</param>
        /// <param name="number">The number to set</param>
        /// <param name="offset">Offset to add to set bit indexes - if 1-9, then use 1, but if 0-8, use 0</param>
        /// <param name="value">The bool value to set the number to</param>
        /// <returns>true if ALL of the numbers in the bitArray are set, false if not</returns>
        public static void Set(this BitArray bitArray, int number, int offset, bool value)
        {
            bitArray.Set(number - offset, value);
        }
    }
}