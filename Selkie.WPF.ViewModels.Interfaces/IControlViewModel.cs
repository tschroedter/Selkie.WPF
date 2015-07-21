using System.Collections.Generic;
using System.Windows.Input;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IControlViewModel : IViewModel
    {
        ICommand StartCommand { get; }
        ICommand StopCommand { get; }
        ICommand ExitCommand { get; }
        bool IsStartEnabled { get; }
        bool IsStopEnabled { get; }
        bool IsExitEnabled { get; }
        IEnumerable <string> TestLines { get; }
        string SelectedTestLine { get; set; }
    }
}