import * as Collections from 'typescript-collections';
import { Cell } from './Cell';

export class Board {
    public validOrders: Collections.Set<number> = new Collections.Set<number>();
    public boardOrder: number = -1;
    public cells: Cell[][] = new Array<Cell[]>();

    /**
     * Ctor for a Sudoku Board using a string of values (0-9 or <space>,1-9).
     *
     * @param definition The string to parse the sudoku board from,
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
            // todo: may want to support hex+'g' for order of 16!
            const cellCount: number = definition.length;
            if (!this.isValidCellCount(cellCount)) {
                throw new Error(`${cellCount} is an invalid cell count.`);
            }

            this.boardOrder = Math.sqrt(cellCount);

            for (let rIndex = 0; rIndex < this.boardOrder; rIndex++) {
                this.cells[rIndex] = new Array<Cell>();
                const rowString: string = definition.slice(rIndex * this.boardOrder, (rIndex + 1) * this.boardOrder);
                this.parseRow(rowString, rIndex);
            }
        } else if (Array.isArray(definition) && typeof definition[0] === 'string') {
            // assume it's a compact string of numeric values
            // todo: may want to support hex+'g' for order of 16!

            // it's an array of rows of strings
            const definitionArray: string[] = definition as string[];
            const cellCount = definitionArray.length;

            if (!this.validOrders.contains(cellCount)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }

            this.boardOrder = cellCount;

            for (let rIndex = 0; rIndex < this.boardOrder; rIndex++) {
                this.cells[rIndex] = new Array<Cell>();
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
                this.cells[rIndex] = new Array<Cell>();

                rValue.forEach((cValue, cIndex) => {
                    this.cells[rIndex][cIndex] = new Cell(cValue);
                });
            });
        } else if (typeof definition === 'number') {
            // it's the order of the sudoku board
            if (!this.validOrders.contains(definition)) {
                throw new Error(`${definition} is an invalid board order.`);
            }

            for (let rIndex: number = 0; rIndex < definition; rIndex++) {
                this.cells[rIndex] = new Array<Cell>();

                for (let cIndex: number = 0; cIndex < definition; cIndex++) {
                    this.cells[rIndex][cIndex] = new Cell();
                }
            }
        } else {
            throw new Error(`${typeof definition} is not supported.`);
        }
    }

    public parseRow(rowString: string, r: number) {
        for (let c = 0; c < this.boardOrder; c++) {
            const cellString: string = rowString.slice(c, c + 1);
            const cellNumber: number = Number(cellString);
            if (isNaN(cellNumber)) {
                this.cells[r][c] = new Cell();
            } else {
                this.cells[r][c] = new Cell(cellNumber);
            }
        }
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
