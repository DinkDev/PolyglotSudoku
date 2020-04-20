import * as Collections from 'typescript-collections';
import { Puzzle } from '../Puzzle';
import { PuzzleSize, PuzzleSizeUtil } from '../PuzzleSize';
import { product2, product3, range } from './IterationUtils';

export class DlxPuzzleSolver {
  public puzzle: Puzzle;

  private x: Collections.Dictionary<[string, [number, number]], Collections.Set<[string, string]>>
    = new Collections.Dictionary<[string, [number, number]], Collections.Set<[string, string]>>();
  private y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>
    = new Collections.Dictionary<[string, string], Array<[string, [number, number]]>>();

  constructor(puzzle: Puzzle) {
    this.puzzle = puzzle;

    const constraintTypes: string[] = ['cell', 'row', 'column', 'box'];
    const valueRange: number[] = range(1, this.puzzle.puzzleSize + 1);
    constraintTypes.forEach(constraint => {
      product2(valueRange, valueRange).forEach(pair => {
        this.x.setValue([constraint, pair], new Collections.Set<[string, string]>());
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

  private cover(
    x: Collections.Dictionary<[string, [number, number]], Collections.Set<[string, string]>>,
    y: Collections.Dictionary<[string, string], Array<[string, [number, number]]>>,
    r: [string, string]): Array<Collections.Set<[string, string]>> {
    throw new Error("Method not implemented.");

    const cols: Array<Collections.Set<[string, string]>> = [];
    y.getValue(r)?.forEach(j =>{

      // TODO: implement cover, and all the rest

    });
  }

}
