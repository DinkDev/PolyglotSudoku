from itertools import product
import math

from SudokuPy.Puzzle import Puzzle
from SudokuPy.solvers.PuzzleSolver import PuzzleSolver


class DlxPuzzleSolver(PuzzleSolver):

    def solve(self):
        """ An efficient Sudoku solver using Algorithm x.

        # """
        # setup constraints
        # there are constraints that:
        # - in each cell must be exactly one number
        # - in each row must be all numbers
        # - in each column must be all numbers
        # - in each box must be all numbers
        x = [s for s in product(['cell', 'row', 'column', 'box'],
                                [v for v in product(range(1, self.puzzle.size + 1), range(1, self.puzzle.size + 1))])]

        box_size = int(math.sqrt(self.puzzle.size))
        y = {}
        for r, c, n in product(range(1, self.puzzle.size + 1), range(1, self.puzzle.size + 1),
                               range(1, self.puzzle.size + 1)):
            b = ((r - 1) // box_size) * box_size + ((c - 1) // box_size) + 1
            y[(f'r{r}c{c}', f'{n}')] = [
                ("cell", (r, c)),
                ("row", (r, n)),
                ("column", (c, n)),
                ("box", (b, n))]
        x, y = self.exact_cover(x, y)

        # load grid
        for i in self.puzzle.grid:
            v = self.puzzle.grid[i]
            if v:
                self.select(x, y, (i, v))

        # solve puzzle, note that multiple solutions is an invalid puzzle
        solution_count = 0
        for solution in self.get_solutions(x, y, []):
            solution_count += 1
            if solution_count > 2:
                return
            # solution is a partial puzzle grid with only the name value pairs for the unknown cells

            # create a result that is a copy of the original puzzle
            result = Puzzle()
            for k, v in self.puzzle.grid.items():
                result.grid[k] = v

            # then add the solution to it
            for (i, n) in solution:
                result.grid[i] = n

            yield result

    @staticmethod
    def exact_cover(x, y):
        x = {j: set() for j in x}  # convert x to a dict via comprehension
        for i, row in y.items():
            for j in row:
                x[j].add(i)
        return x, y

    def get_solutions(self, x, y, solution):
        if not x:
            yield list(solution)
        else:
            c = min(x, key=lambda i: len(x[i]))
            for r in list(x[c]):
                solution.append(r)
                cols = self.select(x, y, r)
                for s in self.get_solutions(x, y, solution):
                    yield s
                self.deselect(x, y, r, cols)
                solution.pop()

    @staticmethod
    def select(x, y, r):
        cols = []
        for j in y[r]:
            for i in x[j]:
                for k in y[i]:
                    if k != j:
                        x[k].remove(i)
            cols.append(x.pop(j))
        return cols

    @staticmethod
    def deselect(x, y, r, cols):
        for j in reversed(y[r]):
            x[j] = cols.pop()
            for i in x[j]:
                for k in y[i]:
                    if k != j:
                        x[k].add(i)
