using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public abstract class BaseMapViewModelMessageHandler <T>
        : SelkieInMemoryBusMessageHandlerAsync <T>,
          IMapViewModelMessageHandler
        where T : class
    {
        protected BaseMapViewModelMessageHandler([NotNull] ISelkieLogger logger,
                                                 [NotNull] ISelkieInMemoryBus bus)
            : base(logger,
                   bus)
        {
        }

        // todo testing
        protected IMapViewModel MapViewModel;

        public void SetMapViewModel(IMapViewModel mapViewModel)
        {
            // todo testing
            if ( MapViewModel == null )
            {
                MapViewModel = mapViewModel;
            }
        }
    }
}