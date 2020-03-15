export enum PuzzleSize {
    Undefined = 0,
    FourByFour = 4,
    NineByNine = 9,
    SixteenBySixteen = 16,
}

export class PuzzleSizeUtil {
    public static isValid(size: number): boolean {
        switch (size) {
            case PuzzleSize.FourByFour:
            case PuzzleSize.NineByNine:
            case PuzzleSize.SixteenBySixteen:
                return true;
            default:
                return false;
        }
    }

    public static toPuzzleSize(size: number): PuzzleSize {
        switch (size) {
            case PuzzleSize.FourByFour:
            case PuzzleSize.NineByNine:
            case PuzzleSize.SixteenBySixteen:
                return size as PuzzleSize;
            default:
                return PuzzleSize.Undefined;
        }
    }
}
