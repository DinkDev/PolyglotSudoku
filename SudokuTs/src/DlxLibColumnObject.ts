import { DataObject } from './DlxLibDataObject';

export class ColumnObject extends DataObject {
    public previousColumnObject: ColumnObject;
    public nextColumnObject: ColumnObject;
    public numberOfRows: number;

    constructor() {
        super(null, -1);
        this.previousColumnObject = this;
        this.nextColumnObject = this;
        this.numberOfRows = 0;
    }

    appendColumnHeader(columnObject: ColumnObject) {
        this.previousColumnObject.nextColumnObject = columnObject;
        columnObject.nextColumnObject = this;
        columnObject.previousColumnObject = this.previousColumnObject;
        this.previousColumnObject = columnObject;
    }

    unlinkColumnHeader() {
        this.nextColumnObject.previousColumnObject = this.previousColumnObject;
        this.previousColumnObject.nextColumnObject = this.nextColumnObject;
    }

    relinkColumnHeader() {
        this.nextColumnObject.previousColumnObject = this;
        this.previousColumnObject.nextColumnObject = this;
    }

    addDataObject(dataObject: DataObject) {
        this.appendToColumn(dataObject);
        this.numberOfRows++;
    }

    unlinkDataObject(dataObject: DataObject) {
        dataObject.unlinkFromColumn();
        this.numberOfRows--;
    }

    relinkDataObject(dataObject: DataObject) {
        dataObject.relinkIntoColumn();
        this.numberOfRows++;
    }

    loopNext(fn: (next: ColumnObject) => void) {
        for (let next = this.nextColumnObject; next !== this; next = next.nextColumnObject) {
            fn(next);
        }
    }
}
