import { solutionGenerator } from './DlxLib';

const internalOnSearchStep = (onSearchStep: any, internalRows: any) =>
    (rowIndices: any) => onSearchStep(internalRows, rowIndices);

const internalOnSolutionFound = (onSolutionFound: any, internalRows: any) =>
    (rowIndices: any) => onSolutionFound(internalRows, rowIndices);

export const solve = (puzzle: string[], onSearchStep: (_internalRows: any, _rowIndices: any) => void, onSolutionFound: (_internalRows: any, _rowIndices: any) => void) => {
    const internalRows = buildInternalRows(puzzle);
    const matrix = buildDlxMatrix(internalRows);
    return solutionGenerator(
        matrix,
        internalOnSearchStep(onSearchStep, internalRows));
};

export const rowIndicesToSolution = (puzzle: string[], internalRows: any[], rowIndices: any[]) => {
    const values = puzzleStringToValues(puzzle);
    const solutionInternalRows = rowIndices.map((rowIndex: any) => internalRows[rowIndex]);
    solutionInternalRows.forEach((internalRow: any) => {
        const { row, col } = internalRow.coords;
        values[row * 9 + col] = String(internalRow.value);
    });
    return valuesToPuzzleString(values);
};

const puzzleStringToValues = (puzzle: string[]) => flatten(puzzle.map((s: string) => s.split('')));

const valuesToPuzzleString = (values: any[]) =>
    INDICES.reduce((acc: string[], n: number) => {
        acc.push(values.slice(n * 9, n * 9 + 9).join(''));
        return acc;
    }, []);

const INDICES = Array.from(Array(9).keys());
const ROWS = INDICES;
const COLS = INDICES;
const DIGITS = INDICES.map(n => n + 1);

const buildInternalRows = (puzzle: string[]) => {
    const seqs = ROWS.map(row =>
        COLS.map(col => {
            const coords = { row, col };
            const initialValue = lookupInitialValue(puzzle, row, col);
            return buildInternalRowsForCell(coords, initialValue);
        }));
    return flatten(flatten(seqs));
};

const flatten = (xss: any[]) => xss.reduce((acc: any[], xs: any[]) => acc.concat(xs), []);

const lookupInitialValue = (puzzle: string[], row: number, col: number) => Number(puzzle[row][col]);

const buildInternalRowsForCell = (coords: any, initialValue: number) => {
    return initialValue
        ? [{ coords, value: initialValue, isInitialValue: true }]
        : DIGITS.map(digit => ({ coords, value: digit, isInitialValue: false }));
};

const buildDlxMatrix = (internalRows: any[]) => internalRows.map((internalRow: any) => buildDlxRow(internalRow));

const buildDlxRow = (internalRow: any) => {
    const { row, col } = internalRow.coords;
    const value = internalRow.value;
    const box = rowColToBox(row, col);
    const posVals = encode(row, col);
    const rowVals = encode(row, value - 1);
    const colVals = encode(col, value - 1);
    const boxVals = encode(box, value - 1);
    const result = posVals.concat(rowVals, colVals, boxVals);
    return result;
};

const rowColToBox = (row: number, col: number) => Math.floor(row - (row % 3) + (col / 3));

const encode = (major: number, minor: number) => {
    const result = Array(81).fill(0);
    result[major * 9 + minor] = 1;
    return result;
};
