namespace SudokuSharp.Engine
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property changed.</param>
        [NotifyPropertyChangedInvocator]
        protected void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}