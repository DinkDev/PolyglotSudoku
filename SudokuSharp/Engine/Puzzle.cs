using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(@"SudokuSharp.Engine.Tests")]

namespace SudokuSharp.Engine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using Annotations;

    /// <summary>
    /// A simple implementation of a Sudoku puzzle!
    /// </summary>
    public sealed class Puzzle : INotifyPropertyChanged
    {
        private byte?[,] _grid; // only access through the wrapper property this[]!
        private PuzzleSize _puzzleSize = PuzzleSize.Undefined;

        /// <summary>
        ///     Default ctor.
        /// </summary>
        public Puzzle() : this(PuzzleSize.Undefined)
        {
            // Must force grid creation since default size is Undefined.
            CreateGrid(PuzzleSize.Undefined);
        }

        /// <summary>
        ///     Public ctor.
        /// </summary>
        /// <param name="puzzleSize">This may not be needed, could be inferred when puzzle is loaded.</param>
        public Puzzle(PuzzleSize puzzleSize)
        {
            Size = puzzleSize;
        }

        private Puzzle(Puzzle other)
        {
            // no null check because it is private.

            LoadPuzzle(other.ToString());
        }

        /// <summary>
        ///     Get the number of cells in the Puzzle
        /// </summary>
        public int CellCount => _grid.Length;

        /// <summary>
        ///     Gets the size of the Puzzle.
        /// </summary>
        public PuzzleSize Size
        {
            get => _puzzleSize;
            private set
            {
                if (value != _puzzleSize)
                {
                    _puzzleSize = value;
                    CreateGrid(_puzzleSize);
                    NotifyOfPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Access the grid via puzzle coordinates
        /// </summary>
        /// <param name="puzzleCoordinate">The coordinate to access</param>
        /// <returns>The cell value at that coordinate if set, null otherwise</returns>
        public byte? this[PuzzleCoordinate puzzleCoordinate]
        {
            get => this[puzzleCoordinate.Row, puzzleCoordinate.Col];
            set => this[puzzleCoordinate.Row, puzzleCoordinate.Col] = value;
        }

        /// <summary>
        ///     Internal property to protect grid for proper status management
        /// </summary>
        /// <param name="row">The row coordinate to access</param>
        /// <param name="column">The column coordinate to access</param>
        /// <returns>The cell value at that coordinate if set, null otherwise</returns>
        internal byte? this[int row, int column]
        {
            get => _grid[row, column];
            set
            {
                var currentValue = _grid[row, column];
                if (!value.Equals(currentValue))
                {
                    _grid[row, column] = value;
                    NotifyOfPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Clone a deep copy of this puzzle.
        /// </summary>
        /// <returns>The cloned puzzle.</returns>
        public Puzzle Clone()
        {
            return new Puzzle(this);
        }

        /// <summary>
        /// Gets all the coordinate keys of the puzzle.
        /// </summary>
        /// <returns>All the coordinates of the puzzle.</returns>
        public IEnumerable<PuzzleCoordinate> GetKeys()
        {
            return from row in Enumerable.Range(0, Size.ToInt32())
                from col in Enumerable.Range(0, Size.ToInt32())
                select new PuzzleCoordinate(row, col);
        }

        /// <summary>
        ///     Gets the PuzzleSize for total cell count in a puzzle grid.
        /// </summary>
        /// <param name="cellCount">The total cell count in a puzzle grid</param>
        /// <param name="throwIfUndefined">if true, throw, else return Undefined - defaults to true</param>
        /// <returns>the PuzzleSize</returns>
        public PuzzleSize InferPuzzleSizeByCellCount(int cellCount, bool throwIfUndefined = true)
        {
            var isValid = PuzzleSizeExtensions.ValidPuzzleSizes
                .Select(v => v.ToInt32() * v.ToInt32())
                .Any(v => v == cellCount);

            if (!isValid && throwIfUndefined)
            {
                throw new ArgumentOutOfRangeException(nameof(cellCount), cellCount, "Not a known grid cell count.");
            }

            return PuzzleSizeExtensions.ToPuzzleSize(Convert.ToInt32(Math.Sqrt(cellCount)));
        }

        /// <summary>
        ///     Takes a string representation of a puzzle and loads it into a puzzle.
        /// </summary>
        /// <param name="puzzleText">A text string to try to load into the puzzle.</param>
        /// <param name="charToCell">char to byte mapping dictionary</param>
        /// <returns>true if loaded, false if not</returns>
        public bool LoadPuzzle(string puzzleText, IDictionary<char, byte?> charToCell = null)
        {
            var rv = false;

            // infer puzzleSize from dimensions
            Size = InferPuzzleSizeByCellCount(puzzleText.Length);
            charToCell ??= Size.ValidCellValues()
                .ToDictionary(v => v < 16 ? $"{v:x}"[0] : 'g', v => (byte?)v);

            var maxCellValue = Size.ToInt32();
            var gridArea = maxCellValue * maxCellValue;

            if (puzzleText.Length == gridArea)
            {
                var rowNum = 0;
                var colNum = 0;
                foreach (var cellChar in puzzleText.ToLowerInvariant())
                {
                    var coord = new PuzzleCoordinate(rowNum, colNum);

                    if (charToCell.ContainsKey(cellChar))
                    {
                        this[coord] = charToCell[cellChar];
                    }
                    else
                    {
                        this[coord] = null;
                    }

                    colNum++;
                    if (colNum >= maxCellValue)
                    {
                        colNum = 0;
                        rowNum++;
                    }

                    rv = true;
                }
            }

            return rv;
        }

        /// <summary>
        ///     Takes an array of strings representation of a puzzle and loads it into a puzzle.
        /// </summary>
        /// <param name="puzzleStrings">An array of strings to try to load into the puzzle.</param>
        /// <returns>true if loaded, false if not</returns>
        public bool LoadPuzzle(string[] puzzleStrings)
        {
            var sb = new StringBuilder();
            foreach (var row in puzzleStrings)
            {
                sb.Append(row);
            }

            return LoadPuzzle(sb.ToString());
        }

        /// <summary>
        ///     Dumps this puzzle as a string.
        /// </summary>
        /// <param name="nullChar"></param>
        /// <param name="newLinePerRow">If true, adds newline characters for each row string</param>
        /// <param name="upperCase">If true, will return all hex-plus characters in uppercase</param>
        /// <returns>This NullableByte puzzle as a string</returns>
        public string ToString(char nullChar = '.', bool newLinePerRow = false, bool upperCase = false)
        {
            var rv = new StringBuilder();
            foreach (var row in this.ByRow())
            {
                if (newLinePerRow && rv.Length != 0)
                {
                    rv.AppendLine();
                }

                foreach (var cell in row)
                {
                    var convertedChar = ConvertToChar(cell.Value, nullChar);
                    convertedChar = upperCase ? convertedChar.ToUpperInvariant() : convertedChar;
                    rv.Append(convertedChar);
                }
            }

            return rv.ToString();
        }

        /// <summary>
        /// Used to convert a cell value to a character (single character string).
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="nullChar"></param>
        /// <returns></returns>
        internal string ConvertToChar(byte? cell, char nullChar = '.')
        {
            // just avoiding a lot of HasValue-Value checks
            var cellValue = cell ?? -1;

            if (cellValue > 0 && cellValue < 16)
            {
                return cellValue.ToString("x");
            }

            if (cellValue == 16)
            {
                return "g";
            }

            return nullChar.ToString();
        }


        /// <summary>
        /// Creates a new grid based upon puzzleSize's int equivalent.
        /// </summary>
        /// <param name="puzzleSize">The size of the puzzle grid to create.</param>
        private void CreateGrid(PuzzleSize puzzleSize)
        {
            var gridDim = puzzleSize != PuzzleSize.Undefined ? puzzleSize.ToInt32() : 0;
            _grid = new byte?[gridDim, gridDim];

            Size = puzzleSize;

            // ReSharper disable once UseNameofExpression
            NotifyOfPropertyChanged(@"Item");
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property changed.</param>
        [NotifyPropertyChangedInvocator]
        private void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}