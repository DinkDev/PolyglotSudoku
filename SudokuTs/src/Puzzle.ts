import * as Collections from 'typescript-collections';
import { PuzzleSize, PuzzleSizeUtil } from './PuzzleSize';

export type cellType = number | null;

export class Puzzle {
    public puzzleSize: PuzzleSize = PuzzleSize.Undefined;
    public grid: cellType[][] = new Array<cellType[]>();

    /**
     * Ctor for a Sudoku Board using a string of values (0-9 or <space>,1-9).
     *
     * @param definition
     *  The string to parse the sudoku board from,
     *  or a string[] representing the rows to parse the sudoku board from,
     *  or a number[][] representing the sudoku board,
     *  or a number representing the order of the sudoku board.
     *
     * @error If the length of definition isn't valid.
     */
    constructor(definition: any) {

        if (typeof definition === 'string') {
            // assume it's a compact string of numeric values
            const cellCount: number = definition.length;

            const size = Math.sqrt(cellCount);

            if (!PuzzleSizeUtil.isValid(size)) {
                throw new Error(`${cellCount} is an invalid cell count.`);
            }

            this.puzzleSize = PuzzleSizeUtil.toPuzzleSize(size);

            for (let rIndex = 0; rIndex < this.puzzleSize; rIndex++) {
                this.grid[rIndex] = new Array<cellType>();
                const rowString: string = definition.slice(rIndex * this.puzzleSize, (rIndex + 1) * this.puzzleSize);
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && typeof definition[0] === 'string') {
            // assume it's an array of rows of strings
            const definitionArray: string[] = definition as string[];

            if (!PuzzleSizeUtil.isValid(definitionArray.length)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }

            this.puzzleSize = definitionArray.length;

            for (let rIndex = 0; rIndex < this.puzzleSize; rIndex++) {
                this.grid[rIndex] = new Array<(number|null)>();
                const rowString: string = definitionArray[rIndex];
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && Array.isArray(definition[0]) && typeof definition[0][0] === 'number') {
            // it's an array of rows of arrays columns of number
            const definitionArray: number[][] = definition as number[][];

            if (!PuzzleSizeUtil.isValid(definitionArray.length)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }

            this.puzzleSize = definitionArray.length;

            definitionArray.forEach((rValue, rIndex) => {
                if (!PuzzleSizeUtil.isValid(rValue.length)) {
                    throw new Error(`Row ${rIndex} has a length of ${rValue.length} that is an invalid board order.`);
                }
                this.grid[rIndex] = new Array<(number|null)>();

                rValue.forEach((cValue, cIndex) => {
                    this.grid[rIndex][cIndex] = this.getCellValue(cValue);
                });
            });
        } else if (typeof definition === 'number') {
            // it's the order of the sudoku board
            if (!PuzzleSizeUtil.isValid(definition)) {
                throw new Error(`${definition} is an invalid board order.`);
            }

            this.puzzleSize = definition as number;

            for (let rIndex: number = 0; rIndex < this.puzzleSize; rIndex++) {
                this.grid[rIndex] = new Array<cellType>();

                for (let cIndex: number = 0; cIndex < this.puzzleSize; cIndex++) {
                    this.grid[rIndex][cIndex] = null;
                }
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
