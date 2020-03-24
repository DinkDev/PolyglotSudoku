import { expect } from 'chai';
import 'mocha';

import { Puzzle } from '../src/Puzzle';
import { PuzzleSize } from '../src/PuzzleSize';

describe('Puzzle constructor', () => {

    it('args === number declares an uninitialized 9 by 9 board', () => {

        const sut = new Puzzle(9);
        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid.getValue('r0c0')).to.equal(null);
        expect(sut.grid.getValue('r0c1')).to.equal(null);
        expect(sut.grid.getValue('r4c4')).to.equal(null);
        expect(sut.grid.getValue('r8c8')).to.equal(null);
    });

    it('args instanceOf Puzzle copies other', () => {
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
        const other = new Puzzle(9);
        other.loadPuzzle(boardDef);

        const sut = new Puzzle(other);
        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid.getValue('r0c0')).to.equal(4);
        expect(sut.grid.getValue('r0c1')).to.equal(null);
        expect(sut.grid.getValue('r4c4')).to.equal(3);
        expect(sut.grid.getValue('r8c8')).to.equal(2);
    });

});

describe('Puzzle.parse', () => {

    it('args === string parses 9 rows of 9 characters', () => {
        const boardDef = '4..27.6..798156234.2.84...7237468951849531726561792843.82.15479.7..243....4.87..2';

        const sut = new Puzzle(9);
        sut.loadPuzzle(boardDef);

        expect(sut.puzzleSize).to.equal(9);
        expect(sut.grid.getValue('r0c0')).to.equal(4);
        expect(sut.grid.getValue('r0c1')).to.equal(null);
        expect(sut.grid.getValue('r4c4')).to.equal(3);
        expect(sut.grid.getValue('r8c8')).to.equal(2);
    });

    it('args === string[] parses 9 rows of 9 characters', () => {
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

        const sut = new Puzzle(9);
        sut.loadPuzzle(boardDef);

        expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        expect(sut.grid.getValue('r0c0')).to.equal(4);
        expect(sut.grid.getValue('r0c1')).to.equal(null);
        expect(sut.grid.getValue('r4c4')).to.equal(3);
        expect(sut.grid.getValue('r8c8')).to.equal(2);
    });

});
