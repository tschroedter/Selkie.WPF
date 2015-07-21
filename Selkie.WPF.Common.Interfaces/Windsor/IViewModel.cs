using System.ComponentModel;

namespace Selkie.WPF.Common.Interfaces.Windsor
{
    public interface IViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}