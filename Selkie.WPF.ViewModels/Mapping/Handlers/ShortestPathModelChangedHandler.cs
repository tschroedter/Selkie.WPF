using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class ShortestPathModelChangedHandler
        : BaseMapViewModelMessageHandler <ShortestPathModelChangedMessage>
    {
        private readonly IShortestPathModel m_ShortestPathModel;

        public ShortestPathModelChangedHandler([NotNull] ISelkieLogger logger,
                                               [NotNull] ISelkieInMemoryBus bus,
                                               [NotNull] IShortestPathModel shortestPathModel)
            : base(logger,
                   bus)
        {
            m_ShortestPathModel = shortestPathModel;
        }

        public override void Handle(ShortestPathModelChangedMessage message)
        {
            var shortestPath = new List <IDisplayLine>();
            shortestPath.AddRange(m_ShortestPathModel.Path);

            MapViewModel.SetshortestPath(shortestPath);
        }
    }
}