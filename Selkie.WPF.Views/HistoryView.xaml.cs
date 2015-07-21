using JetBrains.Annotations;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class HistoryView : ITrailsHistoryView
    {
        public HistoryView([UsedImplicitly] ITrailHistoryViewModel model)
        {
            InitializeComponent();
        }
    }
}