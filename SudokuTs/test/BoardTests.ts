import { expect } from 'chai';
import 'mocha';

import { Board } from '../src/Board';
import { Cell } from '../src/Cell';

describe('Board constructor', () => {

    it('args =  string parses 9 rows of 9 characters', () => {
        const boardDef = '4..27.6..798156234.2.84...7237468951849531726561792843.82.15479.7..243....4.87..2';

        const sut = new Board(boardDef);
        expect(sut.boardOrder).to.equal(9);
        expect(sut.cells[0][0].cellValue).to.equal(4);
        expect(sut.cells[0][0].isGiven).to.equal(true);
        expect(sut.cells[0][1].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[0][1].isGiven).to.equal(false);
        expect(sut.cells[4][4].cellValue).to.equal(3);
        expect(sut.cells[4][4].isGiven).to.equal(true);
        expect(sut.cells[8][8].cellValue).to.equal(2);
        expect(sut.cells[8][8].isGiven).to.equal(true);
    });

    it('args =  string[] parses 9 rows of 9 characters', () => {
        const boardDef = [
            '4..27.6..',
            '798156234',
            '.2.84...7',
            '237468951',
            '849531726',
            '561792843',
            '.82.15479',
            '.7..243..',
            '..4.87..2',
        ];

        const sut = new Board(boardDef);
        expect(sut.boardOrder).to.equal(9);
        expect(sut.cells[0][0].cellValue).to.equal(4);
        expect(sut.cells[0][0].isGiven).to.equal(true);
        expect(sut.cells[0][1].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[0][1].isGiven).to.equal(false);
        expect(sut.cells[4][4].cellValue).to.equal(3);
        expect(sut.cells[4][4].isGiven).to.equal(true);
        expect(sut.cells[8][8].cellValue).to.equal(2);
        expect(sut.cells[8][8].isGiven).to.equal(true);
    });

    it('args =  number[][] parses 9 rows of 9 characters', () => {
        const boardDef = [
            [4, -1, -1, 2, 7, -1, 6, -1, -1],
            [7, 9, 8, 1, 5, 6, 2, 3, 4],
            [-1, 2, -1, 8, 4, -1, -1, -1, 7],
            [2, 3, 7, 4, 6, 8, 9, 5, 1],
            [8, 4, 9, 5, 3, 1, 7, 2, 6],
            [5, 6, 1, 7, 9, 2, 8, 4, 3],
            [-1, 8, 2, -1, 1, 5, 4, 7, 9],
            [-1, 7, -1, -1, 2, 4, 3, -1, -1],
            [-1, -1, 4, -1, 8, 7, -1, -1, 2],
        ];

        const sut = new Board(boardDef);
        expect(sut.boardOrder).to.equal(9);
        expect(sut.cells[0][0].cellValue).to.equal(4);
        expect(sut.cells[0][0].isGiven).to.equal(true);
        expect(sut.cells[0][1].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[0][1].isGiven).to.equal(false);
        expect(sut.cells[4][4].cellValue).to.equal(3);
        expect(sut.cells[4][4].isGiven).to.equal(true);
        expect(sut.cells[8][8].cellValue).to.equal(2);
        expect(sut.cells[8][8].isGiven).to.equal(true);
    });

    it('args =  number declares an uninitialized 9 by 9 board', () => {
        const sut = new Board(9);
        expect(sut.boardOrder).to.equal(9);
        expect(sut.cells[0][0].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[0][0].isGiven).to.equal(false);
        expect(sut.cells[0][1].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[0][1].isGiven).to.equal(false);
        expect(sut.cells[4][4].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[4][4].isGiven).to.equal(false);
        expect(sut.cells[8][8].cellValue).to.equal(Cell.noCellValue);
        expect(sut.cells[8][8].isGiven).to.equal(false);
    });

});
