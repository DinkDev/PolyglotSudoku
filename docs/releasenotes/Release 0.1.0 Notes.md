PolyglotSudoku 0.1.0

I am finally code complete on the first installment of PolyglotSudoku!
In this set, I have created 3 Sudoku samples in C#, Python and Typescript.

Each language implementation has:
* A basic puzzle model that supports:
    * Loading a puzzle from  multiple formats
    * Indexing the puzzle squares by row and column
    * Getting and setting the value of a puzzle square
* A thorough set of unit tests.  The test frameworks used are:
    * For C# - XUnit
    * For Typescript, Mocha and Chai
    * For Python, pytest
    
The C# and Typescript versions have extra support for multiple dimensions:
* 4 x 4
* 9 x 9 (standard Sudoku size)
* and 16 x 16

The Python version only supports 9 x 9, this is mostly dynamically loaded, so it could be extended to the other two sizes.  Maybe this will happen for next release.

The Python sample is a significant refactoring of Peter Norvig's depth first search puzzle solver in python at [http://norvig.com/sudoku.html](http://norvig.com/sudoku.html).  Since this was a solver, I've abstracted a PuzzleSolver base class and Peter's specific implementation.

The next installment will have sophisticated solvers for any puzzle.  One of the reasons that I've decided to make multiple implementations using different programming languages is to explore the differences in how you might use that language to solve the problem.  Another reason is that there are a lot of great open source code samples out there for Sudoku.  The C# allows me to easily adapt .NET pieces, Typescript is a great alternative for ES6 and JavaScript implementations. And of course, Python has been used extensively for Sudoku.