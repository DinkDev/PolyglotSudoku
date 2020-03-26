from Puzzle import Puzzle
import time


def test_init():
    """A set of tests that must pass."""
    p = Puzzle()

    assert len(p.squares) == 81
    assert len(p.unit_list) == 27
    assert all(len(p.units[s]) == 3 for s in p.squares)
    assert all(len(p.peers[s]) == 20 for s in p.squares)

    assert p.units['r3c2'] == [['r1c2', 'r2c2', 'r3c2', 'r4c2', 'r5c2', 'r6c2', 'r7c2', 'r8c2', 'r9c2'],
                               ['r3c1', 'r3c2', 'r3c3', 'r3c4', 'r3c5', 'r3c6', 'r3c7', 'r3c8', 'r3c9'],
                               ['r1c1', 'r1c2', 'r1c3', 'r2c1', 'r2c2', 'r2c3', 'r3c1', 'r3c2', 'r3c3']]

    assert p.peers['r3c2'] == {'r1c2', 'r2c2', 'r4c2', 'r5c2', 'r6c2', 'r7c2', 'r8c2', 'r9c2', 'r3c1', 'r3c3', 'r3c4',
                               'r3c5', 'r3c6', 'r3c7', 'r3c8', 'r3c9', 'r1c1', 'r1c3', 'r2c1', 'r2c3'}

    print('All tests pass.')


def solve_all(grids, name=''):
    """Attempt to solve a sequence of grids. Report results."""
    times, results = zip(*[time_solve(grid) for grid in grids])
    n = len(results)
    if n > 1:
        print("Solved %d of %d %s puzzles (avg %.2f secs (%d Hz), max %.2f secs)." % (
            sum(results), n, name, sum(times) / n, n / sum(times), max(times)))


def time_solve(grid):
    p = Puzzle()
    start = time.perf_counter()
    values = p.solve(grid)
    t = time.perf_counter() - start
    return t, solved(p, values)


def solved(p, values):
    """A puzzle is solved if each unit is a permutation of the digits 1 to 9."""

    def unit_solved(unit): return set(values[s] for s in unit) == set(p.digits)

    return values is not False and all(unit_solved(unit) for unit in p.unit_list)


grid1 = '003020600900305001001806400008102900700000008006708200002609500800203009005010300'
grid2 = '4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......'
hard1 = '.....6....59.....82....8....45........3........6..3.54...325..6..................'


def test_solve_easy():
    solve_all(open("sudoku-easy50.txt"), "easy")


def test_solve_hard():
    solve_all(open("sudoku-top95.txt"), "hard")


def test_solve_hardest():
    solve_all(open("sudoku-hardest.txt"), "hardest")
