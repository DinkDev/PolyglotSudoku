import { expect } from 'chai';
import 'mocha';

import { Puzzle } from '../src/Puzzle';
import { PuzzleSize } from '../src/PuzzleSize';
import { DlxPuzzleSolver } from '../src/solvers/DlxPuzzleSolver';

describe('DlxPuzzleSolver constructor', () => {

    it('args instanceOf Puzzle initializes state', () => {
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
        const puzzle = new Puzzle(9);
        puzzle.loadPuzzle(boardDef);

        const sut = new DlxPuzzleSolver(puzzle);

        expect(sut.puzzle).to.equal(puzzle);
        // tslint:disable-next-line: no-string-literal
        expect(sut['x'].size()).to.equal(141);
        // tslint:disable-next-line: no-string-literal
        expect(sut['y'].size()).to.equal(729);
        // expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        // expect(sut.grid.getValue('r0c0')).to.equal('4');
        // expect(sut.grid.getValue('r0c1')).to.equal(null);
        // expect(sut.grid.getValue('r4c4')).to.equal('3');
        // expect(sut.grid.getValue('r8c8')).to.equal('2');
    });

    it('solve finds a solution', () => {
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
        const puzzle = new Puzzle(9);
        puzzle.loadPuzzle(boardDef);

        const sut = new DlxPuzzleSolver(puzzle);

        const result = sut.solve();

        expect(sut.puzzle).to.equal(puzzle);
        // tslint:disable-next-line: no-string-literal
        expect(sut['x'].size()).to.equal(141);
        // tslint:disable-next-line: no-string-literal
        expect(sut['y'].size()).to.equal(729);
        // expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        // expect(sut.grid.getValue('r0c0')).to.equal('4');
        // expect(sut.grid.getValue('r0c1')).to.equal(null);
        // expect(sut.grid.getValue('r4c4')).to.equal('3');
        // expect(sut.grid.getValue('r8c8')).to.equal('2');
    });

});
