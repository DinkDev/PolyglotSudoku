import { expect } from 'chai';
import 'mocha';

import { solve } from '../src/DlxLibSolver';

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

        // tslint:disable-next-line: no-empty
        const onSearchStep = (_internalRows: any, _rowIndices: any) => {
        };
        // tslint:disable-next-line: no-empty
        const onSolutionFound = (_internalRows: any, _rowIndices: any) => {
        };

        const solutionGenerator = solve(boardDef, onSearchStep, onSolutionFound);
        solutionGenerator.next();
    });


});

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

        // tslint:disable-next-line: no-empty
        const onSearchStep = (_internalRows: any, _rowIndices: any) => {
        };
        // tslint:disable-next-line: no-empty
        const onSolutionFound = (_internalRows: any, _rowIndices: any) => {
        };

        const solutionGenerator = solve(boardDef, onSearchStep, onSolutionFound);
        solutionGenerator.next();
    });


});