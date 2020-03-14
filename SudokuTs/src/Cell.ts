export class Cell {
    public isGiven: boolean;
    public cellValue: number;
    public noCellValue: number = 0; /** normalize values for cell with no value assigned yet */

    /**
     * Overloaded initialization for Cell with a given value.
     *
     * @param givenValue optional given value for a cell.
     */
    constructor(givenValue?: number) {
        if (givenValue && givenValue > 0) {
            this.isGiven = true;
            this.cellValue = givenValue;
        } else {
            this.isGiven = false;
            this.cellValue = this.noCellValue;
        }
    }
}
