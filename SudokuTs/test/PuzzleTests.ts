import { expect } from 'chai';
import 'mocha';

import { Puzzle } from '../src/Puzzle';
import { PuzzleSize } from '../src/PuzzleSize';

describe('Board constructor', () => {

    it('args =  string parses 9 rows of 9 characters', () => {
        const boardDef = '4..27.6..798156234.2.84...7237468951849531726561792843.82.15479.7..243....4.87..2';

        const sut = new Puzzle(boardDef);
        expect(sut.puzzleSize).to.equal(9);
        expect(sut.grid[0][0]).to.equal(4);
        expect(sut.grid[0][1]).to.equal(null);
        expect(sut.grid[4][4]).to.equal(3);
        expect(sut.grid[8][8]).to.equal(2);
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

        const sut = new Puzzle(boardDef);
        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid[0][0]).to.equal(4);
        expect(sut.grid[0][1]).to.equal(null);
        expect(sut.grid[4][4]).to.equal(3);
        expect(sut.grid[8][8]).to.equal(2);
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

        const sut = new Puzzle(boardDef);
        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid[0][0]).to.equal(4);
        expect(sut.grid[0][1]).to.equal(null);
        expect(sut.grid[4][4]).to.equal(3);
        expect(sut.grid[8][8]).to.equal(2);
    });

    it('args =  number declares an uninitialized 9 by 9 board', () => {

        const sut = new Puzzle(PuzzleSize.NineByNine);
        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid[0][0]).to.equal(null);
        expect(sut.grid[0][1]).to.equal(null);
        expect(sut.grid[4][4]).to.equal(null);
        expect(sut.grid[8][8]).to.equal(null);
    });

});
