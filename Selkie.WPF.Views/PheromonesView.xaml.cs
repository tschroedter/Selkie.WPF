using JetBrains.Annotations;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class PheromonesView : IPheromonesView
    {
        public PheromonesView([UsedImplicitly] IPheromonesViewModel model)
        {
            InitializeComponent();
        }
    }
}