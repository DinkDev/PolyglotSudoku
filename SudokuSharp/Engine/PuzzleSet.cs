namespace SudokuSharp.Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Annotations;

    public class PuzzleSet : ISet<byte>
    {
        public PuzzleSet(PuzzleSize size)
        {
            if (!new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            }.Contains(size))
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"Must be a defined {nameof(PuzzleSize)} value");
            }

            MaxValue = size.ToInt32();
            Bits = new BitArray(MaxValue);
        }

        public PuzzleSet([NotNull] PuzzleSet other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            MaxValue = other.MaxValue;
            Bits = new BitArray(other.Bits);
        }

        public PuzzleSet(PuzzleSize size, [NotNull] IEnumerable<byte> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            }.Contains(size))
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"Must be a defined {nameof(PuzzleSize)} value");
            }

            MaxValue = size.ToInt32();
            Bits = new BitArray(MaxValue);

            AddRange(values);
        }

        public BitArray Bits { get; set; }

        public int MaxValue { get; }

        /// <summary>
        /// Gets the number of bits set in the bitArray.
        /// </summary>
        /// <returns>The number of bits set in the bitArray</returns>
        /// <remarks>
        /// This method is extremely fast - faster then Brian Kernighan's technique.
        /// See: http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        /// and http://stackoverflow.com/questions/5063178/counting-bits-set-in-a-net-bitarray-class
        /// </remarks>
        public int Count
        {
            get
            {
                // optimized for never going to have a size above 16
                var intArray = new int[1];
                Bits.CopyTo(intArray, 0);

                // fix for not truncated bits in last integer that may have been set to true with SetAll()
                // C# 8 is pretty cool (and more pythonic)!
                intArray[^1] &= ~(-1 << (Bits.Length % 32));

                var rv = intArray.Single();

                unchecked
                {
                    rv -= ((rv >> 1) & 0x55555555);
                    rv = (rv & 0x33333333) + ((rv >> 2) & 0x33333333);
                    rv = ((rv + (rv >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                return rv;
            }
        }

        public bool IsReadOnly => false;

        public bool Add(byte item)
        {
            RangeCheck(nameof(item), item);

            var rv = !Contains(item);
            if (rv)
            {
                Bits[item - 1] = true;
            }
            return rv;
        }

        public bool AddRange([NotNull] IEnumerable<byte> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var rv = true;

            foreach (var value in values)
            {
                if (!Add(value))
                {
                    rv = false;
                }
            }

            return rv;
        }

        public void Clear()
        {
            Bits = new BitArray(MaxValue);
        }

        public bool Contains(byte item)
        {
            RangeCheck(nameof(item), item);

            return Bits[item - 1];
        }

        private void RangeCheck(string name, byte item)
        {
            if (item < 1)
            {
                throw new ArgumentOutOfRangeException(name, item, $"Must be 1 or more");
            }

            if (item > MaxValue)
            {
                throw new ArgumentOutOfRangeException(name, item, $"Must be {MaxValue} or less");
            }
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            for (byte i = 0; i < MaxValue; i++)
            {
                if (Bits[i])
                {
                    array[arrayIndex++] = Convert.ToByte(i + 1);
                }
            }
        }

        public void ExceptWith(IEnumerable<byte> other)
        {
            if (other is PuzzleSet otherSet
                && otherSet.MaxValue == MaxValue)
            {
                var otherBits = new BitArray(otherSet.Bits);
                Bits.And(otherBits.Not());
            }
            else
            {
                foreach (var value in other)
                {
                    Remove(value);
                }
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (byte i = 1; i <= MaxValue; i++)
            {
                if (Contains(i))
                {
                    yield return i;
                }
            }
        }

        public void IntersectWith(IEnumerable<byte> other)
        {
            if (other is PuzzleSet otherSet
                && otherSet.MaxValue == MaxValue)
            {
                Bits.And(otherSet.Bits);
            }
            else
            {
                var otherList = other.ToList();

                foreach (var value in this)
                {
                    if (!otherList.Contains(value))
                    {
                        Remove(value);
                    }
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<byte> other)
        {
            if (Count == 0)
            {
                return true;
            }

            var otherSet = other as PuzzleSet;
            if (otherSet == null)
            {
                otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
                foreach (var value in other)
                {
                    otherSet.Add(value);
                }
            }

            return this.All(x => otherSet.Contains(x))
                   && otherSet.Any(x => !Contains(x));
        }

        public bool IsProperSupersetOf(IEnumerable<byte> other)
        {
            var otherSet = other as PuzzleSet;
            if (otherSet == null)
            {
                otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
                foreach (var value in other)
                {
                    otherSet.Add(value);
                }
            }

            if (otherSet.Count == 0)
            {
                return true;
            }

            return otherSet.IsProperSubsetOf(this);
        }

        public bool IsSubsetOf(IEnumerable<byte> other)
        {
            if (Count == 0)
            {
                return true;
            }

            var otherSet = other as PuzzleSet;
            if (otherSet == null)
            {
                otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
                foreach (var value in other)
                {
                    otherSet.Add(value);
                }
            }

            return this.All(x => otherSet.Contains(x));
        }

        public bool IsSupersetOf(IEnumerable<byte> other)
        {
            var otherSet = other as PuzzleSet;
            if (otherSet == null)
            {
                otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
                foreach (var value in other)
                {
                    otherSet.Add(value);
                }
            }

            if (otherSet.Count == 0)
            {
                return true;
            }

            return otherSet.IsSubsetOf(this);
        }

        public bool Overlaps(IEnumerable<byte> other)
        {
            return other.Any(Contains);
        }

        public bool Remove(byte item)
        {
            RangeCheck(nameof(item), item);

            var rv = Contains(item);
            if (rv)
            {
                Bits[item - 1] = false;
            }
            return rv;
        }

        public bool SetEquals(IEnumerable<byte> other)
        {
            var otherSet = other as PuzzleSet;

            if (otherSet == null || otherSet.MaxValue != MaxValue)
            {
                otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
                foreach (var value in other)
                {
                    otherSet.Add(value);
                }
            }

            return otherSet.All(Contains) && this.All(x => otherSet.Contains(x));
        }

        public void SymmetricExceptWith(IEnumerable<byte> other)
        {
            var otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
            foreach (var value in other)
            {
                otherSet.Add(value);
            }

            Bits.Xor(otherSet.Bits);
        }

        public void UnionWith(IEnumerable<byte> other)
        {
            var otherSet = new PuzzleSet(PuzzleSizeExtensions.ToPuzzleSize(MaxValue));
            foreach (var value in other)
            {
                otherSet.Add(value);
            }

            Bits.Or(otherSet.Bits);
        }

        void ICollection<byte>.Add(byte item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
