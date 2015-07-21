using JetBrains.Annotations;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class ControlView : IControlView
    {
        public ControlView([UsedImplicitly] IControlViewModel model)
        {
            InitializeComponent();
        }
    }
}