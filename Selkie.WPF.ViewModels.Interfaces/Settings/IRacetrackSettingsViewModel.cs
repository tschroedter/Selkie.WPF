using System.Windows.Input;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces.Settings
{
    public interface IRacetrackSettingsViewModel : IViewModel
    {
        bool IsPortTurnAllowed { get; set; }
        bool IsStarboardTurnAllowed { get; set; }
        ICommand ApplyCommand { get; }
        bool IsApplying { get; }
    }
}