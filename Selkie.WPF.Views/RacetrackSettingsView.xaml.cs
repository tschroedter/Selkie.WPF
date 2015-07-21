using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces.Settings;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class RacetrackSettingsView : ISettingsView
    {
        public RacetrackSettingsView([UsedImplicitly] IRacetrackSettingsViewModel model)
        {
            InitializeComponent();
        }

        public IView ParentView { get; set; }
    }
}