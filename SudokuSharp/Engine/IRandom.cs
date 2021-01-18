namespace SudokuSharp.Engine
{
    using System;

    public interface IRandom
    {
        int GetRandomNumber(int count = int.MaxValue);

        int GetRandomNumber(int min, int max);
    }
}