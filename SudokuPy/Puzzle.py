from typing import Any


def cross(list_a, list_b):
    """Cross product of elements in A and elements in B."""
    return [a + b for a in list_a for b in list_b]


class Puzzle:

    # Solve Every Sudoku Puzzle

    # See http://norvig.com/sudoku.html

    # Throughout this program we have:
    #   r is a row,    e.g. 'r1'
    #   c is a column, e.g. 'c3'
    #   s is a square, e.g. 'r1c3'
    #   d is a digit,  e.g. '9'
    #   u is a unit,   e.g. ['r1c1','r2c1','r3c1','r4c1','r5c1','r6c1','r7c1','r8c1','r9c1']
    #   grid is a grid,e.g. 81 non-blank chars, e.g. starting with '.18...7...
    #   values is a dict of possible values, e.g. {'r1c1':'12349', 'r1c2':'8', ...}

    def __init__(self):
        super().__init__()

        self.digits = '123456789'
        self.rows = ['r' + d for d in self.digits]
        self.cols = ['c' + d for d in self.digits]
        self.squares = cross(self.rows, self.cols)
        self.grid = dict((s, None) for s in self.squares)

    def load_puzzle(self, definition):
        """Convert grid into a dict of {square: char} with '.', ' ' or '0' for empties."""

        # convert list of strings to string
        if isinstance(definition, list):
            flat_grid = ''
            for sub_grid in definition:
                for item in sub_grid:
                    flat_grid += item
            definition = flat_grid

        chars = [c if c in self.digits else None for c in definition]

        if len(chars) != 81:
            print(definition, chars, len(chars))
        assert len(chars) == 81
        for s, d in dict(zip(self.squares, chars)).items():
            self.grid[s] = d
