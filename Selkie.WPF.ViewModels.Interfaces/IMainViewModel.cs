using System.Windows.Input;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IMainViewModel
    {
        ICommand ClosingCommand { get; }
        void ShutDown();
    }
}