namespace SudokuSharp.Engine.Solvers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    public class DifficultPuzzleGenerator
    {
        public DifficultPuzzleGenerator(int order = 3)
        {
            Order = order;
            Size = Order * Order;
        }

        public int Order { get; }
        public int Size { get; }

        /// <summary>
        /// Random number generator (can be set for testing).
        /// </summary>
        public IRandom Random { get; set; } = new RandomHelper();

        public BitArray AllBitsSet()
        {
            return new BitArray(Size, true);
        }

        public bool CellIsSet(byte value)
        {
            return value != 0;
        }

        public int CountSetBits(BitArray value)
        {
            var count = 0;

            if (HasSetBits(value))
            {
                count = value.Cast<bool>().Count(v => v);
            }

            return count;
        }

        public int FindCellWithFewestPossibilities(byte[] puzzle, BitArray[] possibilities)
        {
            var bestIndex = -1;
            var bestScore = -1;

            for (var index = 0; index < puzzle.Length; index++)
            {
                var value = puzzle[index];

                if (!CellIsSet(value))
                {
                    var score = CountSetBits(possibilities[index]);

                    if (bestScore < 0 || score < bestScore)
                    {
                        bestIndex = index;
                        bestScore = score;
                    }
                }
            }

            return bestIndex;
        }

        public BitArray GetSetBit(int value, bool negate = false)
        {
            var rv = new BitArray(Size, negate);
            if (value > 0)
            {
                rv[value - 1] = !negate;
            }
            return rv;
        }

        public bool HasSetBits(BitArray value)
        {
            return value.Cast<bool>().Any(t => t);
        }

        public void EliminatePossibilities(BitArray[] possibilities, int col, int row, int value)
        {
            var mask = GetSetBit(value, negate: true);
            var saved = new BitArray(possibilities[row * Size + col]);

            var b = col;
            for (var i = 0; i < Size; i++)
            {
                possibilities[b].And(mask);
                b += Size;
            }

            b = row * Size;
            for (var i = 0; i < Size; i++)
            {
                possibilities[b + i].And(mask);
            }

            b = (row - row % Order) * Size + col - col % Order;
            for (var i = 0; i < Order; i++)
            {
                for (var j = 0; j < Order; j++)
                {
                    possibilities[b + j].And(mask);
                }

                b += Size;
            }

            possibilities[row * Size + col] = saved;
        }

        public BitArray[] InitializePossibilities(byte[] problem)
        {
            var rv = new BitArray[problem.Length];

            for (var index = 0; index < rv.Length; index++)
            {
                rv[index] = AllBitsSet();
            }

            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    var cellValue = problem[row * Size + col];

                    if (CellIsSet(cellValue))
                    {
                        EliminatePossibilities(rv, col, row, cellValue);
                    }
                }
            }

            return rv;
        }

        public bool PuzzleIsValid(byte[] puzzle, BitArray[] possibilities)
        {
            for (var i = 0; i < puzzle.Length; i++)
            {
                if (CellIsSet(puzzle[i]))
                {
                    if (puzzle[i] == 0 && HasSetBits(possibilities[i])
                        || !possibilities[i][puzzle[i] - 1])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #region AnalysisContext

        public void PossibilityAnalysisForSet(AnalysisContext context, int[] set)
        {
            var count = new int[Size];
            var best = -1;
            var missing = AllBitsSet();

            // Find out what's missing from the set, and how many available
            // slots for each missing number.
            for (var i = 0; i < Size; i++)
            {
                var value = context.Puzzle[set[i]];

                if (CellIsSet(value))
                {
                    missing[value - 1] = false;
                }
                else
                {
                    var possibilities = context.Possibilities[set[i]];
                    for (var j = 0; j < Size; j++)
                    {
                        if (possibilities[j])
                        {
                            count[j]++;
                        }
                    }
                }
            }

            // Look for the missing number with the fewest available slots.
            for (var i = 0; i < Size; i++)
            {
                if (missing[i]
                    && (best < 0 || count[i] < count[best]))
                {
                    best = i;
                }
            }

            // Did we find anything?
            if (best >= 0)
            {
                // If it's better than anything we've found so far, save the result
                if (context.BestSize < 0 || count[best] < context.BestSize)
                {
                    var j = 0;
                    var mask = (ushort)(1 << best);

                    context.BestValue = best + 1;
                    context.BestSize = count[best];

                    for (var i = 0; i < Size; i++)
                    {
                        if (!CellIsSet(context.Puzzle[set[i]])
                            && context.Possibilities[set[i]][best])
                        {
                            context.Best[j++] = set[i];
                        }
                    }
                }
            }
        }

        public int PossibilityAnalysis(byte[] puzzle, BitArray[] possibilities, ref int[] set, out int value)
        {
            var context = new AnalysisContext(puzzle, possibilities, Size);

            for (var i = 0; i < Size; i++)
            {
                set = new int[Size];

                for (var j = 0; j < Size; j++)
                {
                    set[j] = j * Size + i;
                }

                PossibilityAnalysisForSet(context, set);

                for (var j = 0; j < Size; j++)
                {
                    set[j] = i * Size + j;
                }

                PossibilityAnalysisForSet(context, set);

                var b = (i / Order) * Order * Size + (i % Order) * Order;
                for (var j = 0; j < Size; j++)
                {
                    set[j] = b + (j / Order) * Size + j % Order;
                }

                PossibilityAnalysisForSet(context, set);
            }

            Array.Copy(context.Best, set, context.Best.Length);
            value = context.BestValue;
            return context.BestSize;
        }

        public class AnalysisContext
        {
            public AnalysisContext(byte[] puzzle, BitArray[] possibilities, int size = 9)
            {
                Puzzle = puzzle;
                Possibilities = possibilities;
                Best = new int[size];
                BestSize = -1;
                BestValue = -1;
            }

            public byte[] Puzzle { get; }
            public BitArray[] Possibilities { get; }

            public int[] Best { get; }
            public int BestSize { get; set; }
            public int BestValue { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.Append("Puzzle:");
                Array.ForEach(Puzzle, b => sb.Append($" {b},"));
                sb.AppendLine();

                sb.Append("Possibilities:");
                Array.ForEach(Possibilities, v =>
                {
                    var bytes = new byte[2];
                    v.CopyTo(bytes, 0);
                    var value = BitConverter.ToUInt16(bytes);
                    sb.Append($" {value:X4},");
                });
                sb.AppendLine();

                sb.Append("Best:");
                Array.ForEach(Best, b => sb.Append($" {b},"));
                sb.AppendLine();

                sb.AppendLine($"BestSize: {BestSize}");

                sb.AppendLine($"BestValue: {BestValue}");

                return sb.ToString();
            }
        }

        #endregion

        #region Solver

        public void RecursiveSolve(PuzzleSolverContext context, BitArray[] possibilities, int difficulty)
        {
            var cellIndex = FindCellWithFewestPossibilities(context.Problem, possibilities);
            if (cellIndex < 0)
            {
                if (context.Count == 0)
                {
                    context.BranchDifficulty = difficulty;
                    if (context.Solution != null)
                    {
                        Array.Copy(context.Problem, context.Solution, context.Solution.Length);
                    }
                }

                context.Count++;
                return;
            }

            var mask = new BitArray(possibilities[cellIndex]);
            int branchingFactor;

            // If we can't determine a cell value, see if set-oriented
            // backtracking provides a smaller branching factor.
            if (CountSetBits(mask) > 1)
            {
                var offsets = new int[Size];

                var setSize = PossibilityAnalysis(context.Problem, possibilities, ref offsets, out var value);
                if (setSize >= 0 && setSize < CountSetBits(mask))
                {
                    branchingFactor = setSize - 1;
                    difficulty += branchingFactor * branchingFactor;

                    for (var i = 0; i < setSize; i++)
                    {
                        var offset = offsets[i];

                        var newPossibilities = possibilities
                            .Select(x => new BitArray(x))
                            .ToArray();

                        EliminatePossibilities(newPossibilities, offset % Size, offset / Size, value);

                        context.Problem[offset] = Convert.ToByte(value);
                        RecursiveSolve(context, newPossibilities, difficulty);
                        context.Problem[offset] = 0;

                        if (context.Count >= 2)
                        {
                            return;
                        }
                    }

                    return;
                }
            }

            // Otherwise, fall back to cell-oriented backtracking.
            branchingFactor = CountSetBits(mask) - 1;
            difficulty += branchingFactor * branchingFactor;

            for (var i = 0; i < Size; i++)
            {
                if (mask[i])
                {
                    var newPossibilities = possibilities
                        .Select(x => new BitArray(x))
                        .ToArray();

                    EliminatePossibilities(newPossibilities, cellIndex % Size, cellIndex / Size, i + 1);
                    context.Problem[cellIndex] = Convert.ToByte(i + 1);
                    RecursiveSolve(context, newPossibilities, difficulty);

                    if (context.Count >= 2)
                    {
                        return;
                    }
                }
            }

            context.Problem[cellIndex] = 0;
        }

        public int Solve(byte[] puzzle, byte[] solution, out int difficulty)
        {
            difficulty = 0;
            var context = new PuzzleSolverContext(puzzle.ToArray(), solution);

            var possibilities = InitializePossibilities(puzzle);
            if (!PuzzleIsValid(puzzle, possibilities))
            {
                return 0;
            }

            RecursiveSolve(context, possibilities, 0);

            // Calculate a difficulty score
            var empty = 0;
            foreach (var value in puzzle)
            {
                if (!CellIsSet(value))
                {
                    empty++;
                }
            }

            var multiplier = 1;
            while (multiplier <= puzzle.Length)
            {
                multiplier *= 10;
            }

            difficulty = context.BranchDifficulty * multiplier + empty;

            return context.Count;
        }

        public int ActionGeneratePuzzle(PuzzleGenerationOptions options, Action<byte[]> puzzlePrinter)
        {
            //var solution = new byte[Size * Size];

            // TODO: move up to constructor
            var puzzleSize = PuzzleSizeExtensions.ToPuzzleSize(Size);

            var solutionGenerator = new OptimizedPuzzleSolutionGenerator(puzzleSize);

            solutionGenerator.CreatePuzzleSolution();
            var solution = solutionGenerator.PuzzleGrid.ByRow()
                .SelectMany(x => x.Select(c => c.Value ?? 0))
                .ToArray();
            var puzzle = new byte[Size * Size];
            Array.Copy(solution, puzzle, puzzle.Length);

            var difficulty = HardenPuzzle(solution, puzzle, options);

            puzzlePrinter(puzzle);

            Console.WriteLine("Difficulty: {0}", difficulty);
            return 0;
        }

        private int HardenPuzzle(byte[] solution, byte[] puzzle, PuzzleGenerationOptions options)
        {
            var best = 0;
            var cellCount = Size * Size;

            Solve(puzzle, null, out best);

            for (var i = 0; i < options.MaxIterations; i++)
            {
                var next = new byte[puzzle.Length];

                Array.Copy(puzzle, next, next.Length);

                var random = new Random();

                for (var j = 0; j < Size * 2; j++)
                {
                    int c = random.Next() % cellCount;
                    int s = 0;

                    if ((random.Next() & 1) != 0)
                    {
                        next[c] = solution[c];
                        next[cellCount - c - 1] = solution[cellCount - c - 1];
                    }
                    else
                    {
                        next[c] = 0;
                        next[cellCount - c - 1] = 0;
                    }

                    if (Solve(next, null, out s) == 0
                        && s > best
                        && (s <= options.MaxDifficulty || options.MaxDifficulty < 0))
                    {
                        Array.Copy(next, puzzle, puzzle.Length);
                        best = s;

                        if (options.TargetDifficulty >= 0 && s >= options.TargetDifficulty)
                        {
                            return best;
                        }
                    }
                }
            }

            return best;
        }

        public class PuzzleSolverContext
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="problemCopy">The puzzle to solve</param>
            /// <param name="solution">Reference to the solution</param>
            // TODO: should duplicate both problemCopy and solution passed in.
            // TODO: Solve()/SolveRecursive() should get solution back from the context.
            public PuzzleSolverContext(byte[] problemCopy, byte[] solution)
            {
                Problem = problemCopy;
                Solution = solution;
                Count = 0;
                BranchDifficulty = 0;
            }

            public byte[] Problem { get; }
            public int Count { get; set; }
            public byte[] Solution { get; }
            public int BranchDifficulty { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.Append("Problem:");
                Array.ForEach(Problem, b => sb.Append($" {b},"));
                sb.AppendLine();

                sb.AppendLine($"Count: {Count}");

                sb.Append("Solution:");
                Array.ForEach(Solution, b => sb.Append($" {b},"));
                sb.AppendLine();

                sb.AppendLine($"BranchDifficulty: {BranchDifficulty}");

                return sb.ToString();
            }
        }

        #endregion
    }

    public class PuzzleGenerationOptions
    {
        public int MaxIterations { get; set; } = 0;
        public int TargetDifficulty { get; set; } = 0;
        public int MaxDifficulty { get; set; } = 0;
    }
}