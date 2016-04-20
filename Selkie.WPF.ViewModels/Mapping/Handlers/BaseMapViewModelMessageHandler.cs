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
        // todo testing
        protected IMapViewModel MapViewModel;

        protected BaseMapViewModelMessageHandler([NotNull] ISelkieLogger logger,
                                                 [NotNull] ISelkieInMemoryBus bus)
            : base(logger,
                   bus)
        {
        }

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