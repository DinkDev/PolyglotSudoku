from SudokuPy.Puzzle import Puzzle


def test_puzzle_init():
    p = Puzzle()
    assert len(p.squares) == 81