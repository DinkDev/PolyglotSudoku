def cross(list_a, list_b):
    """Creates a  cross product str of 2 list of str.

    Unlike itertools.product, the str parts are concatenated together.

    Args:
        list_a (list of str): A list or rows indices (usually).
        list_b (list of str): A list or columns indices (usually).

    Returns:
        list of str: the corss product of the strings in the two list arguments.    """
    """Cross product of elements in A and elements in B."""
    return [a + b for a in list_a for b in list_b]


class Puzzle:
    """A basic class for a sudoku puzzle

    Uses techniques from Peter Norvig's solver.
    See: http://norvig.com/sudoku.html

    Attributes:
        rows (list of str): sudoku row indices, e.g. 'r1'.
        cols (list of str): sudoku column indices, e.g. 'c3'.
        squares (list of str): sudoku squares indices: e.g. 'r1c3'.
        digits (str): possible square values, '1' - '9.
        grid (dict of str: str): the sudoku puzzle grid or values by square.
    """
    def __init__(self):
        super().__init__()

        self.digits = '123456789'
        self.rows = ['r' + d for d in self.digits]
        self.cols = ['c' + d for d in self.digits]
        self.squares = cross(self.rows, self.cols)
        self.grid = dict((s, None) for s in self.squares)

    def load_puzzle(self, definition):
        """Takes a string or list of strings representation of a puzzle and loads it into the puzzle grid.

        Args:
            definition (str or list of str): the values for the puzzle.

        Returns:
            Nothing.
        """
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
