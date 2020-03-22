import * as Collections from 'typescript-collections';
import { PuzzleSize, PuzzleSizeUtil } from './PuzzleSize';

export type cellType = number | null;

export class Puzzle {
    public puzzleSize: PuzzleSize = PuzzleSize.Undefined;
    public grid: cellType[][] = new Array<cellType[]>();

    public readonly NullChar = '.';

    /**
     * Ctor for a sudoku Puzzle.
     *
     * @param definition
     * null, which just initializes an empty Puzzle,
     * or a number representing the size of the sudoku Puzzle,
     * or another Puzzle to copy
     */
    constructor(definition: any = null) {

        // is it anything
        if (definition !== null) {
            // is it the size
            if (typeof definition === 'number') {
                            // it's the order of the sudoku board
                if (!PuzzleSizeUtil.isValid(definition)) {
                    throw new Error(`${definition} is an invalid board order.`);
                }

                this.puzzleSize = PuzzleSizeUtil.toPuzzleSize(definition as number);
                this.createGrid();

            } else if (definition instanceof Puzzle) {
                this.puzzleSize = definition.puzzleSize;
                this.createGrid();
                this.loadPuzzle(definition.toString());
            }
        }
    }

    public createGrid() {
        for (let rIndex: number = 0; rIndex < this.puzzleSize; rIndex++) {
            this.grid[rIndex] = new Array<cellType>();
            for (let cIndex: number = 0; cIndex < this.puzzleSize; cIndex++) {
                this.grid[rIndex][cIndex] = null;
            }
        }
    }

    /**
     * Takes a string or array of strings representation of a puzzle and loads it into a puzzle dictionary.
     *
     * @param definition
     *  The string to load the sudoku board from,
     *  or a string[] representing the rows to parse the sudoku board from,
     *
     * @error If the length of definition isn't valid.
     */
    public loadPuzzle(definition: any) {

        if (typeof definition === 'string') {
            // assume it's a compact string of numeric values
            const cellCount: number = definition.length;

            const size = this.inferPuzzleSizeByCellCount(cellCount);

            if (size !== this.puzzleSize) {
                this.puzzleSize = size;
                this.createGrid();
            }

            for (let rIndex = 0; rIndex < this.puzzleSize; rIndex++) {
                this.grid[rIndex] = new Array<cellType>();
                const rowString: string = definition.slice(rIndex * this.puzzleSize, (rIndex + 1) * this.puzzleSize);
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && typeof definition[0] === 'string') {
            // assume it's an array of rows of strings
            const definitionArray: string[] = definition as string[];

            const size = this.inferPuzzleSizeByRowLength(definitionArray.length);

            if (size !== this.puzzleSize) {
                this.puzzleSize = size;
                this.createGrid();
            }

            for (let rIndex = 0; rIndex < this.puzzleSize; rIndex++) {
                this.grid[rIndex] = new Array<(number|null)>();
                const rowString: string = definitionArray[rIndex];
                this.parseRow(rowString, rIndex);
            }
        } else {
            throw new Error(`${typeof definition} is not supported.`);
        }
    }

    /**
     * Evaluates cellCount to determine if it is a valid number of cells for a Sudoku board.
     *
     * @param cellCount the count of board cells.
     *
     * @returns true is valid, false otherwise.
     */
    public parseRow(rowString: string, r: number) {

        // todo: may want to support hex+'g' for order of 16!

        for (let c = 0; c < this.puzzleSize; c++) {
            const cellString: string = rowString.slice(c, c + 1);
            const cellNumber: number = Number(cellString);
            if (isNaN(cellNumber)) {
                this.grid[r][c] = null;
            } else {
                this.grid[r][c] = cellNumber;
            }
        }
    }

    /**
     * Converts a puzzle to a string representation.
     */
    public toString(): string {
        let rv: string = '';

        for (let rIndex: number = 0; rIndex < this.puzzleSize; rIndex++) {
            for (let cIndex: number = 0; cIndex < this.puzzleSize; cIndex++) {
                rv += this.convertToChar(this.grid[rIndex][cIndex]);
            }
        }

        return rv;
    }

    /**
     * Converts a Puzzle cell's value to a character.
     *
     * @param cellValue the value of a Puzzle cell
     */
    public convertToChar(cellValue: cellType): string {
        if (cellValue !== null) {
            if (cellValue > 0 && cellValue < 16) {
                return cellValue.toString(16);
            } else if (cellValue === 16) {
                return 'g';
            }
        }
        return this.NullChar;
    }

    /**
     * Gets the PuzzleSize for total cell count in a puzzle grid.
     *
     * @param cellCount: The total cell count in a puzzle grid.
     */
    public inferPuzzleSizeByCellCount(cellCount: number, throwIfUndefined: boolean = true): PuzzleSize {
        const size = Math.sqrt(cellCount);

        if (!PuzzleSizeUtil.isValid(size) && throwIfUndefined) {
            throw new Error(`${cellCount} is an invalid cell count.`);
        }

        return PuzzleSizeUtil.toPuzzleSize(size);
    }

    /**
     * Gets the PuzzleSize from the length of a row in a puzzle grid.
     *
     * @param size: The length of a row in a puzzle grid.
     */
    public inferPuzzleSizeByRowLength(size: number, throwIfUndefined: boolean = true): PuzzleSize {

        if (!PuzzleSizeUtil.isValid(size) && throwIfUndefined) {
            throw new Error(`${size} is an invalid cell count.`);
        }

        return PuzzleSizeUtil.toPuzzleSize(size);
    }

    /**
     * Evaluates boardCellCount to determine if it is a valid number of cells for a Sudoku board.
     *
     * @param boardCellCount the count of board cells.
     *
     * @returns true is valid, false otherwise.
     */
    public isValidCellCount(boardCellCount: number): boolean {
        const validCellCounts = new Collections.Set<number>();
        [PuzzleSize.FourByFour, PuzzleSize.NineByNine, PuzzleSize.SixteenBySixteen].forEach(element => {
            validCellCounts.add(element * element);
        });

        return validCellCounts.contains(boardCellCount);
    }

    /**
     * Evaluates cellCount to determine if it is a valid cell value for a Sudoku board.
     *
     * @param cellValue: the new value for a cell.
     *
     * @returns: the value if valid, null otherwise.
     */
    public getCellValue(cellValue: number): cellType {
        let rv = null;

        // todo: should evaluate if it is in range of the puzzleSize
        if (cellValue > 0) {
            rv = cellValue;
        }
        return rv;
    }
}
