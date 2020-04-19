namespace SudokuSharp.Engine.Solvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dlx;

    /// <summary>
    /// DLX support - inspired by Miran Uhan's Dancing Links Library in CodeProject.
    /// <see cref="http://www.codeproject.com/Articles/19630/Dancing-Links-Library"/>
    /// </summary>
    public class DlxPuzzleSolver
    {
        private PuzzleSize _puzzleSize;
        private readonly Dictionary<int, DlxHeader> _columnHeaders = new Dictionary<int, DlxHeader>();
        private readonly List<DlxNode> _solutionNodes = new List<DlxNode>();

        /// <summary>
        /// The list of solutions found, if any.
        /// </summary>
        /// <remarks>
        /// If more than one puzzle solution is found, then it isn't a valid sudoku puzzle!
        /// </remarks>
        public List<Puzzle> SolutionList { get; } = new List<Puzzle>();

        /// <summary>
        /// A limit on the number of solutions to find - null means no limit.
        /// </summary>
        public int? MaxSolutionsToFind { get; private set; }

        /// <summary>
        /// The initial step to kick off recursive Dlx solver algorithm.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="maxSolutionToFind">The upper limit of the number of solutions to find, if any</param>
        public IList<Puzzle> Solve(Puzzle puzzle, int? maxSolutionToFind = null)
        {
            // setup
            _puzzleSize = puzzle.Size;
            MaxSolutionsToFind = maxSolutionToFind;

            // create/reset
            CreateDlxGrid();

            // load data
            LoadPuzzleGrid(puzzle);

            // solve the puzzle
            RecursiveSolve();

            return SolutionList;
        }

        /// <summary>
        /// Deal with logic to compare an int puzzle value with a nullable byte version.
        /// </summary>
        /// <param name="puzzleValue"></param>
        /// <param name="numericValue"></param>
        /// <returns>true if equal, false otherwise</returns>
        private static bool Compare(byte? puzzleValue, int numericValue)
        {
            var rv = false;

            if (puzzleValue.HasValue)
            {
                rv = puzzleValue.Value == numericValue;
            }

            return rv;
        }

        /// <summary>
        /// Implements the Dlx solution algorithm
        /// </summary>
        private void RecursiveSolve()
        {
            if (_columnHeaders[0].IsSingle())
            {
                SolutionFound(_solutionNodes);
            }
            else
            {
                // Find and remove the next column header from the Dlx grid
                var column = _columnHeaders[0].FindFewestNodes();
                column.Cover();

                // Add each of the column's rows to the solution and remove them from the Dlx grid
                foreach (var row in column.EnumerateList(r => r.Down, MoreToFind))
                {
                    _solutionNodes.Add(row);

                    // Cover the current row - removing this row from the grid
                    foreach (var nextCol in row.EnumerateList(r => r.Right))
                    {
                        nextCol.Header.Cover();
                    }

                    // Recurse
                    RecursiveSolve();

                    // Restore all column headers covered by this row
                    foreach (var nextCol in row.EnumerateList(r => r.Left))
                    {
                        nextCol.Header.Uncover();
                    }

                    // Remove the row from the solution
                    _solutionNodes.Remove(row);
                }

                // Restore the excluded column header
                column.Uncover();
            }
        }

        /// <summary>
        /// Create a puzzle solution out of the Dlx result.
        /// </summary>
        /// <param name="rows">The remaining Dlx nodes containing the solution values.</param>
        private void SolutionFound(IEnumerable<DlxNode> rows)
        {
            var size = _puzzleSize.ToInt32();
            var newSolution = new Dictionary<PuzzleCoordinate, int>();

            foreach (var rowNode in rows)
            {
                if (rowNode != null)
                {
                    var value = rowNode.Coordinate.Row - 1;
                    var digit = value % size + 1;
                    value /= size;
                    var col = value % size;
                    value /= size;
                    var row = value % size;

                    var coord = new PuzzleCoordinate(row, col);
                    newSolution[coord] = digit;
                }
            }

            var solvedPuzzle = new Puzzle(_puzzleSize);

            foreach (var coord in newSolution.Keys.OrderBy(k => k))
            {
                solvedPuzzle[coord] = Convert.ToByte(newSolution[coord]);
            }

            SolutionList.Add(solvedPuzzle);
        }

        /// <summary>
        /// Method to check if MaxSolutionsToFind has been met
        /// </summary>
        /// <returns>true if more to find, false otherwise.</returns>
        private bool MoreToFind()
        {
            // found less than max, or unlimited
            var foundSolutions = SolutionList.Count;
            return foundSolutions < (MaxSolutionsToFind ?? foundSolutions + 1);
        }

        /// <summary>
        /// Create the Dlx grid:
        /// Create the column headers for the grid and chains them
        /// together with the linked list.
        /// </summary>
        private void CreateDlxGrid()
        {
            // reset
            _columnHeaders.Clear();
            _solutionNodes.Clear();
            SolutionList.Clear();

            var primaryColCount = _puzzleSize.ToInt32() * _puzzleSize.ToInt32() * 4;

            // build DLX network
            _columnHeaders[0] = new DlxHeader(0);

            for (var column = 1; column <= primaryColCount; column++)
            {
                _columnHeaders[column] = new DlxHeader(column)
                {
                    Right = null,
                    Left = _columnHeaders[column - 1]
                };

                // Connect the current column to the previous column.
                _columnHeaders[column - 1].Right = _columnHeaders[column];
            }

            // Connect last primary column to the root node.
            _columnHeaders[primaryColCount].Right = _columnHeaders[0];
            _columnHeaders[0].Left = _columnHeaders[primaryColCount];
        }

        /// <summary>
        /// Load the sudoku puzzle with the following 4 constraints
        /// - in each cell must be exactly one number
        /// - in each row must be all numbers
        /// - in each column must be all numbers
        /// - in each box must be all numbers
        /// </summary>
        /// <param name="puzzle">The puzzle to load</param>
        private void LoadPuzzleGrid(Puzzle puzzle)
        {
            var size = puzzle.Size.ToInt32();
            var totalSize = size * size;
            var boxSize = puzzle.Size.BoxSize();
            var givens = new List<DlxNode>();
            var rowCount = 0;
            for (var row = 0; row < size; row++)
            {
                for (var col = 0; col < size; col++)
                {
                    var boxRow = row / boxSize;
                    var boxCol = col / boxSize;
                    for (var digit = 0; digit < size; digit++)
                    {
                        var isGiven = Compare(puzzle[new PuzzleCoordinate(row, col)], digit + 1);
                        var values = new[]
                        {
                            1 + (row * size + col),
                            1 + totalSize + (row * size + digit),
                            1 + 2 * totalSize + (col * size + digit),
                            1 + 3 * totalSize + ((boxRow * boxSize + boxCol) * size + digit)
                        };
                        rowCount++;
                        var newRow = AddNodeRow(values, rowCount);
                        if (isGiven)
                        {
                            givens.Add(newRow);
                        }
                    }
                }
            }

            RemoveGivens(givens);
        }

        /// <summary>
        /// Add a row connecting the nodes to the circular linked nodes for each column.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private DlxNode AddNodeRow(ICollection<int> values, int row)
        {
            DlxNode rv = null;
            DlxNode leftNode = null;

            foreach (var item in values)
            {
                var headerColumn = _columnHeaders[item];
                var newNode = new DlxNode(new PuzzleCoordinate(row, item))
                {
                    Header = headerColumn,
                    Left = leftNode,
                    Right = null
                };

                headerColumn.AddNode(newNode);

                rv ??= newNode;
                rv.Left = newNode;
                newNode.Right = rv;

                if (leftNode != null)
                {
                    leftNode.Right = newNode;
                }

                leftNode = newNode;
            }

            return rv;
        }

        /// <summary>
        /// Remove Dlx rows for the sudoku givens (or the current solved state).
        /// </summary>
        /// <param name="givens">The Dlx nodes for known values to remove Dlx rows for..</param>
        private void RemoveGivens(IEnumerable<DlxNode> givens)
        {
            foreach (var node in givens)
            {
                _solutionNodes.Add(node);
                node.Header.Cover();

                var nextNode = node.Right;
                while (!Equals(nextNode, node))
                {
                    nextNode.Header.Cover();
                    nextNode = nextNode.Right;
                }
            }
        }
    }
}
