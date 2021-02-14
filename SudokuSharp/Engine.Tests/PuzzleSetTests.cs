namespace SudokuSharp.Engine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApprovalTests.Reporters;
    using Xunit;
    using Xunit.Abstractions;

    [UseReporter(typeof(BeyondCompareReporter))]
    public class PuzzleSetTests
    {
        public PuzzleSetTests(ITestOutputHelper helper)
        {
            TestContext = helper;
        }

        public ITestOutputHelper TestContext { get; set; }

        [Fact]
        public void PuzzleSet_Ctor_IsValidSize_Test1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PuzzleSet(PuzzleSize.Undefined));

            var sut = new PuzzleSet(PuzzleSize.FourByFour);
            Assert.Equal(4, sut.MaxValue);

            sut = new PuzzleSet(PuzzleSize.NineByNine);
            Assert.Equal(9, sut.MaxValue);

            sut = new PuzzleSet(PuzzleSize.SixteenBySixteen);
            Assert.Equal(16, sut.MaxValue);
        }

        [Fact]
        public void PuzzleSet_Ctor_BitsInitializes_Test1()
        {
            var sut = new PuzzleSet(PuzzleSize.FourByFour);
            Assert.Equal(4, sut.Bits.Length);

            sut = new PuzzleSet(PuzzleSize.NineByNine);
            Assert.Equal(9, sut.Bits.Length);

            sut = new PuzzleSet(PuzzleSize.SixteenBySixteen);
            Assert.Equal(16, sut.Bits.Length);
        }

        [Fact]
        public void PuzzleSet_AddRangeCheck_Test1()
        {
            var sut = new PuzzleSet(PuzzleSize.FourByFour);
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(5));

             sut = new PuzzleSet(PuzzleSize.NineByNine);
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(10));

             sut = new PuzzleSet(PuzzleSize.SixteenBySixteen);
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Add(17));
        }

        [Fact]
        public void PuzzleSet_AddSyncsWithCount_Test1()
        {
            foreach (var size in new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            })
            {
                var sut = new PuzzleSet(size);
                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.True(sut.Add(value));
                    Assert.Equal(value, sut.Count);
                }
            }
        }

        [Fact]
        public void PuzzleSet_AddSyncsWithCount_Test2()
        {
            foreach (var size in new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            })
            {
                var sut = new PuzzleSet(size);
                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.True(sut.Add(value));
                    Assert.Equal(value, sut.Count);
                }

                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.False(sut.Add(value));
                    Assert.Equal(sut.MaxValue, sut.Count);
                }
            }
        }

        [Fact]
        public void PuzzleSet_Clear_Test1()
        {
            foreach (var size in new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            })
            {
                var sut = new PuzzleSet(size);
                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.True(sut.Add(value));
                    Assert.NotEmpty(sut);
                    Assert.Equal(value, sut.Count);
                }

                sut.Clear();
                Assert.Empty(sut);
            }
        }

        [Fact]
        public void PuzzleSet_Contains_Test1()
        {
            foreach (var size in new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            })
            {
                var sut = new PuzzleSet(size);
                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.True(sut.Add(value));
                    Assert.Contains(value, sut);

                    sut.Clear();
                    Assert.DoesNotContain(value, sut);
                }
            }
        }

        [Fact]
        public void PuzzleSet_CopyTo_Test1()
        {
            foreach (var size in new[]
            {
                PuzzleSize.FourByFour,
                PuzzleSize.NineByNine,
                PuzzleSize.SixteenBySixteen
            })
            {
                var sut = new PuzzleSet(size);
                foreach (var value in Enumerable.Range(1, sut.MaxValue).Select(v => Convert.ToByte(v)))
                {
                    Assert.True(sut.Add(value));
                }

                var actual = new byte[sut.MaxValue];
                sut.CopyTo(actual, 0);

                var expected = Enumerable.Range(1, sut.MaxValue)
                    .Select(v => Convert.ToByte(v))
                    .ToArray();
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void PuzzleSet_ExceptWith_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            for (byte i = 1; i < 6; i++)
            {
                sut1.Add(i);
            }

            for (byte i = 3; i < 10; i++)
            {
                sut2.Add(i);
            }

            sut2.ExceptWith(sut1);

            Assert.Equal(new byte[] {6, 7, 8, 9}, sut2);
        }

        [Fact]
        public void PuzzleSet_GetEnumerator_Test1()
        {
            var sut = new PuzzleSet(PuzzleSize.NineByNine);
            for (byte i = 1; i < 6; i++)
            {
                sut.Add(i);
            }

            var list = new List<byte>(sut);

            Assert.Equal(sut, list);
        }

        [Fact]
        public void PuzzleSet_IntersectWith_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            for (byte i = 1; i < 6; i++)
            {
                sut1.Add(i);
            }

            for (byte i = 3; i < 10; i++)
            {
                sut2.Add(i);
            }

            sut2.IntersectWith(sut1);

            Assert.Equal(new byte[] { 3, 4, 5 }, sut2);
        }

        [Fact]
        public void PuzzleSet_Subset_Superset_Tests1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var allNumbers = new PuzzleSet(PuzzleSize.NineByNine);

            for (byte i = 1; i < 5; i++)
            {
                sut1.Add(i);
            }

            for (byte i = 1; i < 10; i++)
            {
                allNumbers.Add(i);
            }

            TestContext.WriteLine("lowNumbers overlaps allNumbers: {0}",
                sut1.Overlaps(allNumbers));

            TestContext.WriteLine("allNumbers and lowNumbers are equal sets: {0}",
                allNumbers.SetEquals(sut1));

            // Show the results of sub/superset testing
            Assert.True(sut1.IsSubsetOf(allNumbers));
            Assert.True(allNumbers.IsSupersetOf(sut1));
            Assert.True(sut1.IsProperSubsetOf(allNumbers));;
            Assert.True(allNumbers.IsProperSupersetOf(sut1));

            // Modify allNumbers to remove numbers that are not in sut1.
            allNumbers.IntersectWith(sut1);
            
            Assert.True(allNumbers.SetEquals(sut1));

            Assert.True(sut1.IsSubsetOf(allNumbers));
            Assert.True(allNumbers.IsSupersetOf(sut1));
            Assert.False(sut1.IsProperSubsetOf(allNumbers)); ;
            Assert.False(allNumbers.IsProperSupersetOf(sut1));
        }

        [Fact]
        public void PuzzleSet_Overlaps_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut3 = new PuzzleSet(PuzzleSize.NineByNine);

            for (byte value = 1; value <= 3; value++)
            {
                sut1.Add(value);
                sut2.Add(value);
            }

            for (byte value = 4; value <= 6; value++)
            {
                sut2.Add(value);
            }

            for (byte value = 7; value <= 9; value++)
            {
                sut2.Add(value);
                sut3.Add(value);
            }

            Assert.True(sut1.Overlaps(sut2));
            Assert.True(sut2.Overlaps(sut3));
            Assert.False(sut1.Overlaps(sut3));
        }

        [Fact]
        public void PuzzleSet_SetEquals_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut3 = new PuzzleSet(PuzzleSize.NineByNine);

            for (byte value = 1; value <= 3; value++)
            {
                sut1.Add(value);
                sut2.Add(value);
            }

            Assert.True(sut1.SetEquals(sut1));
            Assert.True(sut1.SetEquals(sut2));
            Assert.True(sut2.SetEquals(sut1));

            for (byte value = 4; value <= 6; value++)
            {
                sut2.Add(value);
            }

            for (byte value = 7; value <= 9; value++)
            {
                sut2.Add(value);
                sut3.Add(value);
            }

            Assert.False(sut1.SetEquals(sut2));
            Assert.False(sut2.SetEquals(sut3));
            Assert.False(sut1.SetEquals(sut3));
        }

        [Fact]
        public void PuzzleSet_SymmetricExceptWith_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut3 = new PuzzleSet(PuzzleSize.NineByNine);

            for (byte value = 1; value <= 3; value++)
            {
                sut1.Add(value);
                sut2.Add(value);
            }

            for (byte value = 4; value <= 6; value++)
            {
                sut2.Add(value);
            }

            for (byte value = 7; value <= 9; value++)
            {
                sut2.Add(value);
                sut3.Add(value);
            }

            sut1.SymmetricExceptWith(sut3);
            Assert.Equal(new byte[] {1, 2, 3, 7, 8, 9}, sut1);

            sut1.SymmetricExceptWith(sut2);
            Assert.Equal(new byte[]{4, 5, 6}, sut1);
        }

        [Fact]
        public void PuzzleSet_UnionWith_Test1()
        {
            var sut1 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut2 = new PuzzleSet(PuzzleSize.NineByNine);
            var sut3 = new PuzzleSet(PuzzleSize.NineByNine);

            for (byte value = 1; value <= 3; value++)
            {
                sut1.Add(value);
            }

            for (byte value = 4; value <= 6; value++)
            {
                sut2.Add(value);
            }

            for (byte value = 7; value <= 9; value++)
            {
                sut3.Add(value);
            }

            sut1.UnionWith(sut3);
            Assert.Equal(new byte[] { 1, 2, 3, 7, 8, 9 }, sut1);

            sut1.UnionWith(sut2);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, sut1);
        }
    }
}
