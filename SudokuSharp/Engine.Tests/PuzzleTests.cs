namespace SudokuSharp.Engine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Xunit;
    using Xunit.Abstractions;

    [UseReporter(typeof(BeyondCompareReporter))]
    public class PuzzleTests
    {
        public PuzzleTests(ITestOutputHelper helper)
        {
            TestContext = helper;
        }

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public ITestOutputHelper TestContext { get; set; }

        [Fact]
        public void Puzzle_DefaultCtor_Test1()
        {
            var sut = new Puzzle();

            Assert.Equal(PuzzleSize.Undefined, sut.Size);
            Assert.Equal(0, sut.CellCount);
        }

        [Fact]
        public void Puzzle_CtorTest_UndefinedDoesNotThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Puzzle(PuzzleSize.Undefined);
        }

        [Fact]
        public void Puzzle_CtorTest_PuzzleSize_FourByFour()
        {
            var sut = new Puzzle(PuzzleSize.FourByFour);

            Assert.Equal(PuzzleSize.FourByFour, sut.Size);
            Assert.Equal(4, sut.Size.ToInt32());
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            const int expectedCount = 16;
            var cellCount = (from row in sut.ByRow()
                from cell in row
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from col in sut.ByCol()
                from cell in col
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from box in sut.ByBox()
                from cell in box
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);

            Assert.Equal(expectedCount, sut.CellCount);
        }

        [Fact]
        public void Puzzle_CtorTest_PuzzleSize_NineByNine()
        {
            var sut = new Puzzle(PuzzleSize.NineByNine);

            Assert.Equal(PuzzleSize.NineByNine, sut.Size);
            Assert.Equal(9, sut.Size.ToInt32());
            // Assert.Equal(PuzzleStatus.InProgress, sut.PuzzleStatus);
            Assert.Equal(9, sut.ByBox().Count());
            const int expectedCount = 81;
            var cellCount = (from row in sut.ByRow()
                from cell in row
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from col in sut.ByCol()
                from cell in col
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from box in sut.ByBox()
                from cell in box
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
        }

        [Fact]
        public void Puzzle_CtorTest_PuzzleSize_SixteenBySixteen()
        {
            var sut = new Puzzle(PuzzleSize.SixteenBySixteen);

            Assert.Equal(PuzzleSize.SixteenBySixteen, sut.Size);
            Assert.Equal(16, sut.Size.ToInt32());
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());
            Assert.Equal(16, sut.ByBox().Count());
            const int expectedCount = 256;
            var cellCount = (from row in sut.ByRow()
                from cell in row
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from col in sut.ByCol()
                from cell in col
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from box in sut.ByBox()
                from cell in box
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
        }

        // Note: all further testing will just be done using 9x9 size

        [Fact]
        public void Puzzle_CoordinatesForByRow_AreCorrect()
        {
            var sut = new Puzzle(PuzzleSize.NineByNine);

            var rowNum = 0;
            foreach (var row in sut.ByRow())
            {
                var colNum = 0;
                foreach (var cell in row)
                {
                    Assert.Equal(rowNum, cell.Coordinate.Row);
                    Assert.Equal(colNum, cell.Coordinate.Col);
                    colNum++;
                }

                rowNum++;
            }
        }

        [Fact]
        public void Puzzle_CoordinatesForByCol_AreCorrect()
        {
            var sut = new Puzzle(PuzzleSize.NineByNine);

            var colNum = 0;
            foreach (var col in sut.ByCol())
            {
                var rowNum = 0;
                foreach (var cell in col)
                {
                    Assert.Equal(rowNum, cell.Coordinate.Row);
                    Assert.Equal(colNum, cell.Coordinate.Col);
                    rowNum++;
                }

                colNum++;
            }
        }

        [Fact]
        public void Puzzle_CoordinatesForByBox_AreCorrect()
        {
            var sut = new Puzzle(PuzzleSize.NineByNine);

            var boxNum = 0;
            foreach (var row in sut.ByBox())
            {
                var rowCellNum = 0;
                foreach (var cell in row)
                {
                    var rowNum = (boxNum / 3) * 3 + rowCellNum / 3;
                    var colNum = (boxNum % 3) * 3 + rowCellNum % 3;

                    TestContext.WriteLine($"{cell.Coordinate.Row}, {cell.Coordinate.Col}");

                    Assert.Equal(rowNum, cell.Coordinate.Row);
                    Assert.Equal(colNum, cell.Coordinate.Col);
                    rowCellNum++;
                }

                boxNum++;
            }
        }

        [Fact]
        public void Puzzle_InternalIndex_Test1()
        {
            var sut = new Puzzle();

            // too small
            Assert.Throws<IndexOutOfRangeException>(() => sut[0, 0]);
        }

        [Fact]
        public void Puzzle_InternalIndex_Test2()
        {
            var sut = new Puzzle(PuzzleSize.SixteenBySixteen);

            // too large
            Assert.Throws<IndexOutOfRangeException>(() => sut[16, 16]);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_FourByFour_Test1()
        {
            var puzzleStrings = new[]
            {
                "12..",
                "..1.",
                ".3..",
                "..34"
            };
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);

            var sut = new Puzzle(PuzzleSize.FourByFour);

            var result = sut.LoadPuzzle(puzzleStrings);
            var resultLines = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualLines, resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_SixteenBySixteen_Test1()
        {
            var puzzleStrings = new[]
            {
                "96B..G.8..415..3",
                "8.....C..F2..76A",
                "5....6138.7..GB.",
                "..1..D2.5..9.84.",
                ".5.F...A.6...1..",
                "..9GE4..FC....5.",
                "A2.....17.8B3..F",
                "E...2.6.3....4C7",
                "CG48.3..A.E...1.",
                "..5.8...B...2..D",
                ".B.2.7..4D.CG...",
                "D....A..1..87B..",
                "B..D..8..5.4C.32",
                "6..4...G9.B..A8.",
                "2..1.C.6D.FG.9.B",
                "F.3.A.D9..C24..."
            };
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);

            var sut = new Puzzle(PuzzleSize.SixteenBySixteen);

            var result = sut.LoadPuzzle(puzzleStrings);
            var resultLines = sut.AsText(newLinePerRow: true, upperCase: true);

            TestContext.WriteLine(resultLines);

            Assert.True(result);
            Assert.Equal(actualLines, resultLines);

            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_Test1()
        {
            var puzzleStrings = GetPuzzle1();
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(puzzleStrings);
            var resultLines = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualLines, resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_Test2()
        {
            var puzzleStrings = GetPuzzle1();
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);
            var puzzleText = puzzleStrings.Aggregate((current, next) => current + next);

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(puzzleText);
            var resultLines = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualLines, resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_Test3()
        {
            var puzzleStrings = GetPuzzle1();
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);
            var puzzleText = puzzleStrings.Aggregate((current, next) => current + next);
            var sb = new StringBuilder();

            foreach (var itemChar in puzzleText)
            {
                sb.Append(itemChar == '.' ? ' ' : itemChar);
            }

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(sb.ToString());
            var resultLines = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualLines, resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_LoadPuzzle_Translate_Test()
        {
            var puzzleStrings = GetPuzzle1();
            var puzzleText = puzzleStrings.Aggregate((current, next) => current + next);

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(puzzleText, GetPuzzle1Translation());

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            CheckPuzzle1Translated(sut);
        }

        [Fact]
        public void Puzzle_Clone_Test1()
        {
            var actualLines = GetPuzzle1();
            var orig = new Puzzle(PuzzleSize.NineByNine);
            var result = orig.LoadPuzzle(actualLines);

            var sut = orig.Clone();

            Assert.Equal(PuzzleSize.NineByNine, sut.Size);
            Assert.Equal(9, sut.Size.ToInt32());
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());
            Assert.Equal(9, sut.ByBox().Count());
            const int expectedCount = 81;
            var cellCount = (from row in sut.ByRow()
                from cell in row
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from col in sut.ByCol()
                from cell in col
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);
            cellCount = (from box in sut.ByBox()
                from cell in box
                select cell).Count();
            Assert.Equal(expectedCount, cellCount);

            var resultLines = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.Equal(actualLines.Aggregate((c, n) => c + Environment.NewLine + n), resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Assert.True(sut.ComparePuzzle(orig));

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_AsText_Test1()
        {
            var puzzleStrings = GetPuzzle1();
            var actualText = puzzleStrings.Aggregate((current, next) => current + next);

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(puzzleStrings);
            var resultText = sut.AsText();

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualText, resultText);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_AsText_Test2()
        {
            var puzzleStrings = GetPuzzle1();
            var actualLines = puzzleStrings.Aggregate((current, next) => current + Environment.NewLine + next);

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(puzzleStrings);
            var resultText = sut.AsText(newLinePerRow: true);

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(actualLines, resultText);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_AsText_Test3()
        {
            var puzzleStrings = GetPuzzle1();

            var puzzleText = puzzleStrings.Aggregate((current, next) => current + next);
            var sb = new StringBuilder();

            foreach (var itemChar in puzzleText)
            {
                sb.Append(itemChar == '.' ? ' ' : itemChar);
            }

            var sut = new Puzzle(PuzzleSize.NineByNine);

            var result = sut.LoadPuzzle(sb.ToString());
            var resultLines = sut.AsText(' ');

            TestContext.WriteLine(sut.AsText(newLinePerRow: true));

            Assert.True(result);
            Assert.Equal(sb.ToString(), resultLines);
            Assert.Equal(PuzzleStatus.InProgress, sut.GetPuzzleStatus());

            Approvals.Verify(sut);
        }

        [Fact]
        public void Puzzle_InferPuzzleSizeByCellCount_Test1()
        {
            var sut = new Puzzle();

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.InferPuzzleSizeByCellCount(1234));
        }

        [Fact]
        public void Puzzle_PropertyChangedIsNotified_test1()
        {
            var sut = new Puzzle();
            var wasSet = new List<string>();
            sut.PropertyChanged += (o,  a) => wasSet.Add(a.PropertyName);

            sut.LoadPuzzle(GetPuzzle1());

            Assert.Equal(29, wasSet.Count);
        }

        private string[] GetPuzzle1()
        {
            var puzzleStrings = new[]
            {
                "6...5.3.1",
                "4597.....",
                ".1..9.4..",
                ".......6.",
                "...849...",
                ".7.......",
                "..3.7..1.",
                ".....5637",
                "7.1.2...8"
            };
            return puzzleStrings;
        }

        /// <summary>
        /// All values are shifted up one (and 9 is now 1)
        /// </summary>
        private IDictionary<char, byte?> GetPuzzle1Translation()
        {
            return new Dictionary<char, byte?>
            {
                {'1', 2},
                {'2', 3},
                {'3', 4},
                {'4', 5},
                {'5', 6},
                {'6', 7},
                {'7', 8},
                {'8', 9},
                {'9', 1}
            };
        }

        /// <summary>
        /// All values are shifted up one (and 9 is now 1)
        /// </summary>
        /// <param name="sut"></param>
        private void CheckPuzzle1Translated(Puzzle sut)
        {
            Assert.Equal(7, sut[new PuzzleCoordinate(0, 0)] ?? -1);
            Assert.Equal(6, sut[new PuzzleCoordinate(0, 4)] ?? -1);
            Assert.Equal(4, sut[new PuzzleCoordinate(0, 6)] ?? -1);
            Assert.Equal(2, sut[new PuzzleCoordinate(0, 8)] ?? -1);

            Assert.Equal(5, sut[new PuzzleCoordinate(1, 0)] ?? -1);
            Assert.Equal(6, sut[new PuzzleCoordinate(1, 1)] ?? -1);
            Assert.Equal(1, sut[new PuzzleCoordinate(1, 2)] ?? -1);
            Assert.Equal(8, sut[new PuzzleCoordinate(1, 3)] ?? -1);

            Assert.Equal(2, sut[new PuzzleCoordinate(2, 1)] ?? -1);
            Assert.Equal(1, sut[new PuzzleCoordinate(2, 4)] ?? -1);
            Assert.Equal(5, sut[new PuzzleCoordinate(2, 6)] ?? -1);

            Assert.Equal(7, sut[new PuzzleCoordinate(3, 7)] ?? -1);

            Assert.Equal(9, sut[new PuzzleCoordinate(4, 3)] ?? -1);
            Assert.Equal(5, sut[new PuzzleCoordinate(4, 4)] ?? -1);
            Assert.Equal(1, sut[new PuzzleCoordinate(4, 5)] ?? -1);

            Assert.Equal(8, sut[new PuzzleCoordinate(5, 1)] ?? -1);

            Assert.Equal(4, sut[new PuzzleCoordinate(6, 2)] ?? -1);
            Assert.Equal(8, sut[new PuzzleCoordinate(6, 4)] ?? -1);
            Assert.Equal(2, sut[new PuzzleCoordinate(6, 7)] ?? -1);

            Assert.Equal(6, sut[new PuzzleCoordinate(7, 5)] ?? -1);
            Assert.Equal(7, sut[new PuzzleCoordinate(7, 6)] ?? -1);
            Assert.Equal(4, sut[new PuzzleCoordinate(7, 7)] ?? -1);
            Assert.Equal(8, sut[new PuzzleCoordinate(7, 8)] ?? -1);

            Assert.Equal(8, sut[new PuzzleCoordinate(8, 0)] ?? -1);
            Assert.Equal(2, sut[new PuzzleCoordinate(8, 2)] ?? -1);
            Assert.Equal(3, sut[new PuzzleCoordinate(8, 4)] ?? -1);
            Assert.Equal(9, sut[new PuzzleCoordinate(8, 8)] ?? -1);
        }
    }
}
