using System.Windows.Input;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.ViewModels.Interfaces.Settings
{
    public interface IAntSettingsViewModel : IViewModel
    {
        bool IsFixedStartNode { get; set; }
        bool IsApplying { get; }
        bool IsApplyEnabled { get; }
        ICommand ApplyCommand { get; }
        IAntSettingsNode SelectedNode { get; }
    }
}