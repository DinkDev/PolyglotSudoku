from abc import ABCMeta, abstractmethod
from typing import Any
from SudokuPy.Puzzle import Puzzle, cross


class PuzzleSolver(metaclass=ABCMeta):
    """Base class for sudoku puzzle solvers.

    Uses techniques from Peter Norvig's solver.
    See: http://norvig.com/sudoku.html

    Attributes:
        puzzle (Puzzle): the puzzle being solved.
        unit_list (list of str): every combination of rows, columns and boxes.
        units (dict of str: list of str): every unit_list combination for a square
        peers (dict of str: set of str): every square in a unit for a square (without the square itself).
    """
    def __init__(self, p: Puzzle):
        """Initializer.

        Sets all the attributes of the class.

        Args:
            p (Puzzle): the Puzzle to be solved.
        """
        super().__init__()

        self.puzzle = p
        self.unit_list = ([cross(self.puzzle.rows, [c]) for c in self.puzzle.cols]
                          + [cross([r], self.puzzle.cols) for r in self.puzzle.rows]
                          + [cross(rs, cs) for rs in (['r1', 'r2', 'r3'], ['r4', 'r5', 'r6'], ['r7', 'r8', 'r9'])
                             for cs in (['c1', 'c2', 'c3'], ['c4', 'c5', 'c6'], ['c7', 'c8', 'c9'])])
        self.units = dict((s, [u for u in self.unit_list if s in u]) for s in self.puzzle.squares)
        self.peers = dict((s, set(sum(self.units[s], [])) - {s}) for s in self.puzzle.squares)

    @abstractmethod
    def solve(self):
        """Solves the puzzle.

        Returns:
             dict of str: str: the solved puzzle.
        """
        pass

    def get_pencil_marks(self):
        """Finds the possible values for each square of a puzzle.

        Returns (dict of str: str): the possible values for each square, or False for a detected error.
        """
        # To start, every square can be any digit; then assign values from the grid.
        values = dict((s, self.puzzle.digits) for s in self.puzzle.squares)
        d: Any
        for s, d in self.puzzle.grid.items():
            if d is not None and d in self.puzzle.digits and not self.assign(values, s, d):
                return False  # (Fail if we can't assign d to square s.)
        return values

    def assign(self, values, s, d):
        """Eliminate all other values from square s other than value d.

        Args:
            values (dict of str: str): the dictionary of all possible values for all squares.
            s (str): the square to assign the value to.
            d (str): the value for the square.

        Returns:
            dict of str: str: the puzzle possible values by square, after d is assigned.
        """
        other_values = values[s].replace(d, '')
        if all(self.eliminate(values, s, d2) for d2 in other_values):
            return values
        else:
            return False

    def eliminate(self, values, s, d):
        """Eliminate d from values, propagating the change to each unit and peer.

        Args:
            values (dict of str: str): the dictionary of all possible values for all squares.
            s (str): the square to assign the value to.
            d (str): the value for the square.

        Returns:
            dict of str: str: the puzzle possible values by square, after d assignment is propagated,
            or false if an error is detected.
        """
        if d not in values[s]:
            return values  # Already eliminated
        values[s] = values[s].replace(d, '')
        # (1) If a square s is reduced to one value d2, then eliminate d2 from the peers.
        if len(values[s]) == 0:
            return False  # Contradiction: removed last value
        elif len(values[s]) == 1:
            d2 = values[s]
            if not all(self.eliminate(values, s2, d2) for s2 in self.peers[s]):
                return False
        # (2) If a unit u is reduced to only one place for a value d, then put it there.
        for u in self.units[s]:
            dplaces = [s for s in u if d in values[s]]
            if len(dplaces) == 0:
                return False  # Contradiction: no place for this value
            elif len(dplaces) == 1:
                # d can only be in one place in unit; assign it there
                if not self.assign(values, dplaces[0], d):
                    return False
        return values

    def display(self, values, puzzle: Puzzle):
        """Display the puzzle values as a 2-D grid.

        Args:
            values (dict of str: str): the dictionary of the solution to puzzle.
            puzzle: (Puzzle): the puzzle being solved

        Returns:
            None
        """
        width = 1 + max(len(values[s]) for s in puzzle.squares)
        line = '+'.join(['-' * (width * 3)] * 3)
        for r in self.puzzle.rows:
            print(''.join(values[r + c].center(width) + ('|' if c in '36' else '') for c in puzzle.cols))
            if r in 'CF':
                print(line)
        print()
