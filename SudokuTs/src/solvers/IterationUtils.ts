export type optionalNumber = number | undefined;

export function range(start: number, stop?: number, step?: number): number[] {
    if (typeof stop === 'undefined') {
        // one param defined
        stop = start;
        start = 0;
    }
    if (typeof step === 'undefined') {
        step = 1;
    }
    if ((step > 0 && start >= stop) || (step < 0 && start <= stop)) {
        return [];
    }
    const result = [];
    for (let i = start; step > 0 ? i < stop : i > stop; i += step) {
        result.push(i);
    }
    return result;
}

export function product2<T1, T2>(range1: T1[], range2: T2[]): Array<[T1, T2]> {
    const result: Array<[T1, T2]> = [];
    range1.forEach(r1 => {
        range2.forEach(r2 => {
            result.push([r1, r2])
        });
    });

    return result;
}

export function product3<T1, T2, T3>(range1: T1[], range2: T2[], range3: T3[]): Array<[T1, T2, T3]> {
    const result: Array<[T1, T2, T3]> = [];
    range1.forEach(r1 => {
        range2.forEach(r2 => {
            range3.forEach(r3 => {
                result.push([r1, r2, r3])
            });
        });
    });

    return result;
}