import * as Collections from 'typescript-collections';
import { Cell } from './Cell';

class Board {
    public validOrders: Collections.Set<number> = new Collections.Set<number>();
    public boardOrder: number;
    public cells: Cell[][];

    /**
     * Ctor for a Sudoku Board using a string of values (0-9 or <space>,1-9).
     *
     * @param definition The string to parse the sudoku board from,
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
            // todo: may want to support hex for order of 16!
            const cellCount: number = definition.length;
            if (!this.isValidCellCount(cellCount)) {
                throw new Error(`${cellCount} is an invalid cell count.`);
            }

            this.boardOrder = Math.sqrt(cellCount);

            for (let r = 0; r < this.boardOrder; r++) {
                const rowString: string = definition.slice(r * this.boardOrder, (r + 1) * this.boardOrder);
                for (let c = 0; c < this.boardOrder; c++) {
                    const cellString: string = rowString.slice(0, this.boardOrder);
                    const cellNumber: number = Number(cellString);
                    if (Number.isNaN(cellNumber)) {
                        this.cells[r][c] = new Cell();
                    } else {
                        this.cells[r][c] = new Cell(cellNumber);
                    }
                }
            }
        } else if (Array.isArray(definition) && Array.isArray(definition[0] && typeof definition[0][0] === 'number')) {
            // it's an array of rows or arrays columns of number
            const definitionArray: number[][] = definition as number[][];

            if (!this.validOrders.contains(definitionArray.length)) {
                throw new Error(`${definitionArray.length} is an invalid board order.`);
            }
            this.boardOrder = definitionArray.length;

            definitionArray.forEach((rValue, rIndex) => {
                if (!this.validOrders.contains(rValue.length)) {
                    throw new Error(`Row ${rIndex} has a length of ${rValue.length} that is an invalid board order.`);
                }

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
                for (let cIndex: number = 0; cIndex < definition; cIndex++) {
                    this.cells[rIndex][cIndex] = new Cell();
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
    public isValidCellCount(cellCount: number): boolean {
        const validCellCounts = new Collections.Set<number>();
        this.validOrders.forEach(element => {
            validCellCounts.add(element * element);
        });

        return validCellCounts.contains(cellCount);
    }
}
