from PuzzleSolver import PuzzleSolver


class NorvigPuzzleSolver(PuzzleSolver):

    def solve(self):
        """Solves the puzzle.

        Solves the puzzle by using Peter Norvig's depth first search technique.
        See: http://norvig.com/sudoku.html

        Returns:
            dict of str: str: the solved puzzle values by square.
        """
        return self.search(self.get_pencil_marks())

    def search(self, values):
        """Depth first search sudoku solver.

        Brute force sudoku puzzle solver that tries all values.

        Args:
            values (dict of str: str): the dictionary of all possible values for all squares.

        Returns:
             dict of str: str: the solved puzzle.
        """
        if values is False:
            return False  # Failed earlier
        if all(len(values[s]) == 1 for s in self.puzzle.squares):
            return values  # Solved!
        # Chose the unfilled square s with the fewest possibilities
        n, s = min((len(values[s]), s) for s in self.puzzle.squares if len(values[s]) > 1)
        for d in values[s]:
            result = self.search(self.assign(values.copy(), s, d))
            if result:
                return result
