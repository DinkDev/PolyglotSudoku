import { expect } from 'chai';
import 'mocha';

import { Board } from '../src/Board';

describe('constructor option string[]', () => {
    it('should parse 9 rows of 9 characters', () => {
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
    });
});
