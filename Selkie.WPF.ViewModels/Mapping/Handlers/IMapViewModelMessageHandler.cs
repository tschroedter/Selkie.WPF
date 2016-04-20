using JetBrains.Annotations;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public interface IMapViewModelMessageHandler
    {
        void SetMapViewModel([NotNull] IMapViewModel mapViewModel);
    }
}