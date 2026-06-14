type DirectionProp = 'up' | 'down' | 'left' | 'right';

export class DataObject {
    public up: DataObject;
    public down: DataObject;
    public left: DataObject;
    public right: DataObject;
    public listHeader: DataObject | null;
    public rowIndex: number;

    constructor(listHeader: DataObject | null, rowIndex: number) {
        this.listHeader = listHeader;
        this.rowIndex = rowIndex;
        this.up = this;
        this.down = this;
        this.left = this;
        this.right = this;
        if (listHeader) {
            (listHeader as any).addDataObject(this);
        }
    }

    appendToRow(dataObject: DataObject) {
        this.left.right = dataObject;
        dataObject.right = this;
        dataObject.left = this.left;
        this.left = dataObject;
    }

    appendToColumn(dataObject: DataObject) {
        this.up.down = dataObject;
        dataObject.down = this;
        dataObject.up = this.up;
        this.up = dataObject;
    }

    unlinkFromColumn() {
        this.down.up = this.up;
        this.up.down = this.down;
    }

    relinkIntoColumn() {
        this.down.up = this;
        this.up.down = this;
    }

    loopUp(fn: (next: DataObject) => void) { this.loop(fn, 'up'); }
    loopDown(fn: (next: DataObject) => void) { this.loop(fn, 'down'); }
    loopLeft(fn: (next: DataObject) => void) { this.loop(fn, 'left'); }
    loopRight(fn: (next: DataObject) => void) { this.loop(fn, 'right'); }

    loop(fn: (next: DataObject) => void, propName: DirectionProp) {
        for (let next: DataObject = this[propName]; next !== this; next = next[propName]) {
            fn(next);
        }
    }
}
