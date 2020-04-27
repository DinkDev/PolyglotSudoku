import * as Collections from 'typescript-collections';
import { Puzzle } from '../Puzzle';
import { PuzzleSize, PuzzleSizeUtil } from '../PuzzleSize';
import { product2, product3, range } from './IterationUtils';

export class DlxPuzzleSolver {
    public puzzle: Puzzle;

    private x: Collections.Dictionary<[string, [number, number]], Set<[string, string]>>
        = new Collections.Dictionary<[string, [number, number]], Set<[string, string]>>();
    private y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>
        = new Collections.Dictionary<[string, string], Array<[string, [number, number]]>>();

    constructor(puzzle: Puzzle) {
        this.puzzle = puzzle;

        const constraintTypes: string[] = ['cell', 'row', 'column', 'box'];
        const valueRange: number[] = range(1, this.puzzle.puzzleSize + 1);
        constraintTypes.forEach(constraint => {
            product2(valueRange, valueRange).forEach(pair => {
                this.x.setValue([constraint, pair], new Set<[string, string]>());
            });
        });

        const boxSize: number = Math.sqrt(this.puzzle.puzzleSize);
        product3(valueRange, valueRange, valueRange).forEach(triple => {
            const [r, c, n] = triple;

            const yKey: [string, string] = [`r${r}c${c}`, `${n}`];
            const b = Math.floor((r - 1) / boxSize) * boxSize + Math.floor((c - 1) / boxSize) + 1;

            const yValue: Array<[string, [number, number]]>
                = [['cell', [r, c]], ['row', [r, n]], ['column', [c, n]], ['box', [b, n]]];

            this.y.setValue(yKey, yValue);
        });

        this.y.keys().forEach(i => {
            const row = this.y.getValue(i);
            row?.forEach(j => {
                this.x.getValue(j)?.add(i);
            });
        });

        // load the puzzle
        this.puzzle.grid.keys().forEach(i => {
            const v = this.puzzle.grid.getValue(i);
            if (v !== null && v !== undefined) {
                this.cover(this.x, this.y, [i, v]);
            }
        });
    }

    public solve(): Puzzle[] {
        const solutions = new Array<Puzzle>();

        for (const solution of this.recursiveSolve(this.x, this.y, new Array<[string, string]>())) {
                if (solutions.length >= 2) {
                    return solutions;
                }
                // solution is a partial puzzle grid with only the name value pairs for the unknown cells

                // create a result that is a copy of the original puzzle
                const result = new Puzzle(this.puzzle);

                // then add the solution to it
                for (const s in solution) {
                    if (s !== undefined) {
                        const [i, n] = s;
                        result.grid.setValue(i, n);
                    }
                }

                solutions.push(result);
            }

        return solutions;
    }

    private *recursiveSolve(
        x: Collections.Dictionary<[string, [number, number]], Set<[string, string]>>,
        y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>,
        solution: Array<[string, string]>): Generator<Array<[string, string]>> {

        if (x.isEmpty()) {
            yield solution;
        } else {
            // find smallest set in x

            let minSetSize = this.puzzle?.puzzleSize ?? 9;
            // this finds smallest set size
            // for (const p in x.keys()) {
            //     if (x.getValue(p) !== undefined) {
            //         const thisSize = x.getValue(p)?.size ?? 0;
            //         if (thisSize < minSetSize) {
            //             minSetSize = thisSize;
            //         }
            //     }
            // }

            x.keys().forEach( p => {
                const nextSize = x.getValue(p)?.size ?? 9;
                if (nextSize < minSetSize) {
                    minSetSize = nextSize;
                }
            });

            let c: [string, [number, number]] | undefined;

            x.keys().some(i => {
                if (x.getValue(i)?.size === minSetSize) {
                    c = i;
                    return true;
                }
            });

            if (c !== undefined) {
                const xSets: Array<[string, string]>|undefined = [...x.getValue(c)];
                if (xSets !== undefined) {
                    // tslint:disable-next-line: prefer-for-of
                    for (let i = 0; i < xSets.length; i++) {
                        const r = xSets[i];
                        solution.push(r);
                        // put the covered columns on the stack
                        const cols = this.cover(x, y, r);
                        // tslint:disable-next-line: forin
                        const values = [...this.recursiveSolve(x, y, solution)];
                        // tslint:disable-next-line: prefer-for-of
                        for (let j = 0; j < values.length; j++) {
                            yield values[j];
                        }
                        this.uncover(x, y, r, cols);
                        solution.pop();
                    }
                }
            }
        }
    }

    private cover(
        x: Collections.Dictionary<[string, [number, number]], Set<[string, string]>>,
        y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>,
        r: [string, string]): Array<Set<[string, string]>> {

        const cols: Array<Set<[string, string]>> = [];
        y.getValue(r)?.forEach(j => {
            x.getValue(j)?.forEach(i => {
                y.getValue(i)?.forEach(k => {
                    if (JSON.stringify(k) !== JSON.stringify(j)) {
                        x.getValue(k)?.delete(i);
                    }

                    const v = x.getValue(j);
                    x.remove(j);
                    if (v !== undefined) {
                        cols.push(v);
                    }
                });
            });
        });

        return cols;
    }

    private uncover(
        x: Collections.Dictionary<[string, [number, number]], Set<[string, string]>>,
        y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>,
        r: [string, string],
        cols: Array<Set<[string, string]>>) {

        y.getValue(r)?.slice().reverse().forEach(j => {
            const v = cols.pop();
            if (v !== undefined) {
                x.setValue(j, v);
            }

            x.getValue(j)?.forEach(i => {
                y.getValue(i)?.forEach(k => {
                    if (JSON.stringify(k) !== JSON.stringify(j)) {
                        x.getValue(k)?.add(i);
                    }
                });
            });
        });

        return cols;
    }
}
