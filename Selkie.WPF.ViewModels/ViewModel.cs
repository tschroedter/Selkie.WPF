using System.ComponentModel;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels
{
    public abstract class ViewModel
        : IViewModel,
          INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged(this,
                                new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}