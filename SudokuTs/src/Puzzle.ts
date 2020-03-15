import * as Collections from 'typescript-collections';
import { cell } from './cell';

export class Puzzle {
    public validOrders: Collections.Set<number> = new Collections.Set<number>();
    public boardOrder: number = -1;
    public grid: cell[][] = new Array<cell[]>();

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
        [4, 9, 16].forEach(value => {
                    this.validOrders.add(value);
        });

        if (typeof definition === 'string') {
            // assume it's a compact string of numeric values
            const cellCount: number = definition.length;

            if (!this.isValidCellCount(cellCount)) {
                throw new Error(`${cellCount} is an invalid cell count.`);
            }

            this.boardOrder = Math.sqrt(cellCount);

            for (let rIndex = 0; rIndex < this.boardOrder; rIndex++) {
                this.grid[rIndex] = new Array<cell>();
                const rowString: string = definition.slice(rIndex * this.boardOrder, (rIndex + 1) * this.boardOrder);
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && typeof definition[0] === 'string') {
            // assume it's an array of rows of strings
            const definitionArray: string[] = definition as string[];

            if (!this.validOrders.contains(definitionArray.length)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }

            this.boardOrder = definitionArray.length;

            for (let rIndex = 0; rIndex < this.boardOrder; rIndex++) {
                this.grid[rIndex] = new Array<(number|null)>();
                const rowString: string = definitionArray[rIndex];
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && Array.isArray(definition[0]) && typeof definition[0][0] === 'number') {
            // it's an array of rows of arrays columns of number
            const definitionArray: number[][] = definition as number[][];

            if (!this.validOrders.contains(definitionArray.length)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }

            this.boardOrder = definitionArray.length;

            definitionArray.forEach((rValue, rIndex) => {
                if (!this.validOrders.contains(rValue.length)) {
                    throw new Error(`Row ${rIndex} has a length of ${rValue.length} that is an invalid board order.`);
                }
                this.grid[rIndex] = new Array<(number|null)>();

                rValue.forEach((cValue, cIndex) => {
                    this.grid[rIndex][cIndex] = this.GetCellValue(cValue);
                });
            });
        } else if (typeof definition === 'number') {
            // it's the order of the sudoku board
            if (!this.validOrders.contains(definition)) {
                throw new Error(`${definition} is an invalid board order.`);
            }

            this.boardOrder = definition as number;

            for (let rIndex: number = 0; rIndex < this.boardOrder; rIndex++) {
                this.grid[rIndex] = new Array<cell>();

                for (let cIndex: number = 0; cIndex < this.boardOrder; cIndex++) {
                    this.grid[rIndex][cIndex] = null;
                }
            }
        } else {
            throw new Error(`${typeof definition} is not supported.`);
        }
    }

    public parseRow(rowString: string, r: number) {
        // todo: may want to support hex+'g' for order of 16!

        for (let c = 0; c < this.boardOrder; c++) {
            const cellString: string = rowString.slice(c, c + 1);
            const cellNumber: number = Number(cellString);
            if (isNaN(cellNumber)) {
                this.grid[r][c] = null;
            } else {
                this.grid[r][c] = cellNumber;
            }
        }
    }

    private GetCellValue(cValue: number) {
        let cellValue = null;
        if (typeof (cValue) === 'number' && cValue > 0) {
            cellValue = cValue;
        }
        return cellValue;
    }

    /**
     * Evaluates cellCount to determine if it is a valid number of cells for a Sudoku board.
     *
     * @param cellCount the count of board cells.
     *
     * @returns true is valid, false otherwise.
     */
    public isValidCellCount(cellCount: number): boolean {
        const validCellCounts = new Collections.Set<number>();
        this.validOrders.forEach(element => {
            validCellCounts.add(element * element);
        });

        return validCellCounts.contains(cellCount);
    }
}
