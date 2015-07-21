using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class MapView : IMapView
    {
        public MapView([UsedImplicitly] IMapViewModel model)
        {
            InitializeComponent();
        }

        public IView ParentView { get; set; }
    }
}