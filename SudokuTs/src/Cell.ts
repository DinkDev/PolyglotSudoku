export class Cell {
    public static noCellValue: number = 0; /** normalize values for cell with no value assigned yet */
    public isGiven: boolean;
    public cellValue: number;

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
            this.cellValue = Cell.noCellValue;
        }
    }
}
