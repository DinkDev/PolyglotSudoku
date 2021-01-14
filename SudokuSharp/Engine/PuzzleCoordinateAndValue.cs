namespace SudokuSharp.Engine
{
    using Annotations;

    /// <summary>
    /// Provides ref semantics for all of the puzzle.ByX() methods.
    /// </summary>
    public class PuzzleCoordinateAndValue : NotifyPropertyChangedBase
    {
        private byte? _value;

        [UsedImplicitly]
        public PuzzleCoordinateAndValue()
        {
            Coordinate = new PuzzleCoordinate(-1, -1);
        }

        public PuzzleCoordinateAndValue(PuzzleCoordinate coordinate, byte? value)
        {
            Coordinate = coordinate;
            Value = value;
        }

        public PuzzleCoordinate Coordinate { get; set; }

        public byte? Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;
                    NotifyOfPropertyChanged();
                }
            }
        }
    }
}
