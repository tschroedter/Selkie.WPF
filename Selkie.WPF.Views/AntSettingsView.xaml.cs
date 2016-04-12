using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces.Settings;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class AntSettingsView : IAntSettingsView
    {
        public AntSettingsView([UsedImplicitly] IAntSettingsViewModel model)
        {
            InitializeComponent();
        }

        public IView ParentView { get; set; }
    }
}