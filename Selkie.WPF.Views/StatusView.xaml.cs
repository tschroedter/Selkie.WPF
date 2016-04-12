using JetBrains.Annotations;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class StatusView : IStatusView
    {
        public StatusView([UsedImplicitly] IStatusViewModel model)
        {
            InitializeComponent();
        }
    }
}