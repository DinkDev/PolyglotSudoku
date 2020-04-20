from itertools import product
import math
from typing import Dict, List, Tuple, Union

from SudokuPy.Puzzle import Puzzle
from SudokuPy.solvers.PuzzleSolver import PuzzleSolver


class DlxPuzzleSolver(PuzzleSolver):
    """
    DLX support - inspired by Ali Assaf's Algorithm X in 30 lines! [1]
    [1]: URL https://www.cs.mcgill.ca/~aassaf9/python/algorithm_x.html
    """
    def __init__(self, p: Puzzle):
        """Initializer.

        Sets up the DLX constraints
        there are constraints that:
        - in each cell must be exactly one number
        - in each row must be all numbers
        - in each column must be all numbers
        - in each box must be all numbers

        Args:
            p (Puzzle): the Puzzle to be solved.
        """
        super().__init__(p)

        self.x = {c: set() for c in product(
            ['cell', 'row', 'column', 'box'],
            [v for v in product(range(1, self.puzzle.size + 1), range(1, self.puzzle.size + 1))])}

        box_size = int(math.sqrt(self.puzzle.size))
        self.y = {}
        for r, c, n in product(range(1, self.puzzle.size + 1),
                               range(1, self.puzzle.size + 1),
                               range(1, self.puzzle.size + 1)):
            b = ((r - 1) // box_size) * box_size + ((c - 1) // box_size) + 1
            self.y[(f'r{r}c{c}', f'{n}')] = [
                ("cell", (r, c)),
                ("row", (r, n)),
                ("column", (c, n)),
                ("box", (b, n))]

        for i, row in self.y.items():
            for j in row:
                self.x[j].add(i)

        # load grid
        for i in self.puzzle.grid:
            v = self.puzzle.grid[i]
            if v:
                self.cover(self.x, self.y, (i, v))

    def solve(self) -> Union[Puzzle, None]:
        """Main entry point for using DLX to solve a sudoku.

        Note that multiple solutions is an invalid puzzle!

        Returns:
            One or more solutions in a Puzzle
        """
        solution_count = 0
        for solution in self.recursive_solve(self.x, self.y, []):
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

    def recursive_solve(self,
                        x: Dict[Tuple[str, Tuple[int, int]], set],
                        y: Dict[Tuple[str, str], List[Tuple[str, Tuple[int, int]]]],
                        solutions: List[Tuple[str, str]]) -> Union[List[Tuple[str, str]], None]:
        """ Implements the DLX solution algorithm

        Args:
            x: The DLX nodes
            y: The DLX row headers
            solutions: Buffer for solutions

        Returns:
            Solutions from deeper recursion
        """
        if not x:
            yield list(solutions)
        else:
            c = min(x, key=lambda i: len(x[i]))
            for r in list(x[c]):
                solutions.append(r)
                # put the covered columns on the stack
                cols = self.cover(x, y, r)
                for s in self.recursive_solve(x, y, solutions):
                    yield s
                self.uncover(x, y, r, cols)
                solutions.pop()

    @staticmethod
    def cover(x: Dict[Tuple[str, Tuple[int, int]], set],
              y: Dict[Tuple[str, str], List[Tuple[str, Tuple[int, int]]]],
              r: Tuple[str, str]) -> List[set]:
        """ DLX cover method - try a solution

        Args:
            x: The DLX nodes
            y: The DLX row headers
            r: The DLX row to eliminate

        Returns:
            The DLX nodes from the column eliminated
        """
        cols = []
        for j in y[r]:
            for i in x[j]:
                for k in y[i]:
                    if k != j:
                        x[k].remove(i)
            cols.append(x.pop(j))
        return cols

    @staticmethod
    def uncover(x: Dict[Tuple[str, Tuple[int, int]], set],
                y: Dict[Tuple[str, str], List[Tuple[str, Tuple[int, int]]]],
                r: Tuple[str, str],
                cols: List[set]):
        """ DLX uncover method - restores a row header and column nodes to the model.

        Args:
            x: The DLX nodes
            y: The DLX row headers
            r: The DLX row to restore
            cols: The DLX nodes from the column restored

        Returns:
            None
        """
        for j in reversed(y[r]):
            x[j] = cols.pop()
            for i in x[j]:
                for k in y[i]:
                    if k != j:
                        x[k].add(i)
