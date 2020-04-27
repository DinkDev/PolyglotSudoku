import { expect } from 'chai';
import 'mocha';

import { Puzzle } from '../src/Puzzle';
import { PuzzleSize } from '../src/PuzzleSize';
import { DlxPuzzleSolver } from '../src/solvers/DlxPuzzleSolver';

describe('DlxPuzzleSolver constructor', () => {

    it('args instanceOf Puzzle initializes state', () => {
        const boardDef = [
            '4.....8.5',
            '.3.......',
            '...7.....',
            '.2.....6.',
            '....8.4..',
            '....1....',
            '...6.3.7.',
            '5..2.....',
            '1.4......',
        ];
        const puzzle = new Puzzle(9);
        puzzle.loadPuzzle(boardDef);

        const sut = new DlxPuzzleSolver(puzzle);

        expect(sut.puzzle).to.equal(puzzle);
        expect(sut['x'].size()).to.equal(256);
        expect(sut['y'].size()).to.equal(729);
    });

    it('solve finds a solution', () => {
        const boardDef = [  // same ad grid2d in python
            '4.....8.5',
            '.3.......',
            '...7.....',
            '.2.....6.',
            '....8.4..',
            '....1....',
            '...6.3.7.',
            '5..2.....',
            '1.4......',
        ];
        const puzzle = new Puzzle(9);
        puzzle.loadPuzzle(boardDef);

        const sut = new DlxPuzzleSolver(puzzle);

        const result = sut.solve();

        expect(result.length).to.equal(1)
        // expect(sut.puzzle).to.equal(puzzle);
        // tslint:disable-next-line: no-string-literal
        // expect(sut['x'].size()).to.equal(141);
        // tslint:disable-next-line: no-string-literal
        // expect(sut['y'].size()).to.equal(729);
        // expect(sut.puzzleSize).to.equal(PuzzleSize.NineByNine);
        // expect(sut.grid.getValue('r0c0')).to.equal('4');
        // expect(sut.grid.getValue('r0c1')).to.equal(null);
        // expect(sut.grid.getValue('r4c4')).to.equal('3');
        // expect(sut.grid.getValue('r8c8')).to.equal('2');
    });

});
