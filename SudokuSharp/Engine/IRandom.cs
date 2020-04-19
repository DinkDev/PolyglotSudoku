namespace SudokuSharp.Engine
{
    public interface IRandom
    {
        int GetRandomNumber(int count);

        int GetRandomNumber(int min, int max);
    }
}