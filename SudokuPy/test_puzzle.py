from Puzzle import Puzzle
from NorvigPuzzleSolver import NorvigPuzzleSolver
import time


def solve_all(definitions, name=''):
    """Attempt to solve a sequence of grids. Report results."""

    times, results = zip(*[time_solve(definition) for definition in definitions])
    n = len(results)
    if n > 1:
        print("Solved %d of %d %s puzzles (avg %.2f secs (%d Hz), max %.2f secs)." % (
            sum(results), n, name, sum(times) / n, n / sum(times), max(times)))


def time_solve(definition):
    p = Puzzle()
    p.load_puzzle(definition)

    start = time.perf_counter()

    s = NorvigPuzzleSolver(p)
    values = s.solve()

    t = time.perf_counter() - start

    return t, solved(p, s, values)


def solved(puzzle: Puzzle, solver: NorvigPuzzleSolver, values):
    """A puzzle is solved if each unit is a permutation of the digits 1 to 9."""
    def unit_solved(unit): return set(values[s] for s in unit) == set(puzzle.digits)

    return values is not False and all(unit_solved(unit) for unit in solver.unit_list)


grid1 = '003020600900305001001806400008102900700000008006708200002609500800203009005010300'
grid1b = '  3 2 6  9  3 5  1  18 64    81 29  7       8  67 82    26 95  8  2 3  9  5 1 3  '
grid2 = '4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......'
grid2d = [
    '4.....8.5',
    '.3.......',
    '...7.....',
    '.2.....6.',
    '....8.4..',
    '....1....',
    '...6.3.7.',
    '5..2.....',
    '1.4......',
]
hard1 = '.....6....59.....82....8....45........3........6..3.54...325..6..................'


def test_puzzle_init():

    p = Puzzle()
    assert len(p.squares) == 81


def test_puzzle_solver_init():
    puzzle = Puzzle()
    solver = NorvigPuzzleSolver(puzzle)
    assert len(solver.unit_list) == 27
    assert all(len(solver.units[s]) == 3 for s in puzzle.squares)
    assert all(len(solver.peers[s]) == 20 for s in puzzle.squares)

    assert solver.units['r3c2'] == [['r1c2', 'r2c2', 'r3c2', 'r4c2', 'r5c2', 'r6c2', 'r7c2', 'r8c2', 'r9c2'],
                                    ['r3c1', 'r3c2', 'r3c3', 'r3c4', 'r3c5', 'r3c6', 'r3c7', 'r3c8', 'r3c9'],
                                    ['r1c1', 'r1c2', 'r1c3', 'r2c1', 'r2c2', 'r2c3', 'r3c1', 'r3c2', 'r3c3']]

    assert solver.peers['r3c2'] == {'r1c2', 'r2c2', 'r4c2', 'r5c2', 'r6c2', 'r7c2', 'r8c2', 'r9c2', 'r3c1', 'r3c3',
                                    'r3c4', 'r3c5', 'r3c6', 'r3c7', 'r3c8', 'r3c9', 'r1c1', 'r1c3', 'r2c1', 'r2c3'}


def test_grid1():
    solve_all([grid1], "grid1")


def test_grid1b():
    solve_all([grid1b], "grid1b")


def test_grid2():
    solve_all([grid2], "grid2")


def test_grid2d():
    solve_all([grid2], "grid2d")


def test_hard1():
    solve_all([hard1], "hard1")


def test_solve_easy():
    with open('sudoku-easy50.txt') as f:
        easy_puzzles = [line.rstrip('\n') for line in f]
        solve_all(easy_puzzles, "easy")


def test_solve_hard():
    with open('sudoku-top95.txt') as f:
        hard_puzzles = [line.rstrip('\n') for line in f]
        solve_all(hard_puzzles, "hard")


def test_solve_hardest():
    with open('sudoku-hardest.txt') as f:
        hardest_puzzles = [line.rstrip('\n') for line in f]
        solve_all(hardest_puzzles, "hardest")
