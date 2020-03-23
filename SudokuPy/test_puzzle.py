import pytest
from Puzzle import Puzzle

def test_init():
    "A set of tests that must pass."
    p = Puzzle()

    assert len(p.squares) == 81
    assert len(p.unitlist) == 27
    assert all(len(p.units[s]) == 3 for s in p.squares)
    assert all(len(p.peers[s]) == 20 for s in p.squares)
    assert p.units['C2'] == [['A2', 'B2', 'C2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2'],
                             ['C1', 'C2', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9'],
                             ['A1', 'A2', 'A3', 'B1', 'B2', 'B3', 'C1', 'C2', 'C3']]
    assert p.peers['C2'] == set(['A2', 'B2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2',
                               'C1', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9',
                               'A1', 'A3', 'B1', 'B3'])
    print('All tests pass.')

import time

def solve_all(grids, name=''):
    """Attempt to solve a sequence of grids. Report results."""
    times, results = zip(*[time_solve(grid) for grid in grids])
    N = len(results)
    if N > 1:
        print("Solved %d of %d %s puzzles (avg %.2f secs (%d Hz), max %.2f secs)." % (
            sum(results), N, name, sum(times)/N, N/sum(times), max(times)))
            
def time_solve(grid):
    p = Puzzle()
    start = time.perf_counter()
    values = p.solve(grid)
    t = time.perf_counter()-start
    return (t, solved(p, values))

def solved(p, values):
    "A puzzle is solved if each unit is a permutation of the digits 1 to 9."
    def unitsolved(unit): return set(values[s] for s in unit) == set(p.digits)
    return values is not False and all(unitsolved(unit) for unit in p.unitlist)


grid1  = '003020600900305001001806400008102900700000008006708200002609500800203009005010300'
grid2  = '4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......'
hard1  = '.....6....59.....82....8....45........3........6..3.54...325..6..................'
    
def test_solve_easy():
    solve_all(open("sudoku-easy50.txt"), "easy")

def test_solve_hard():
    solve_all(open("sudoku-top95.txt"), "hard")

def test_solve_hardest():
    solve_all(open("sudoku-hardest.txt"), "hardest")