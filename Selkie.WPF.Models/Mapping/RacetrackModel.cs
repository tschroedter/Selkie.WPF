using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class RacetrackModel : IRacetrackModel
    {
        private readonly ISelkieInMemoryBus m_MemoryBus;
        private readonly IPathToRacetracksConverter m_PathToRacetracksConverter;

        public RacetrackModel([NotNull] ISelkieInMemoryBus memoryBus,
                              [NotNull] IPathToRacetracksConverter pathToRacetracksConverter)
        {
            m_MemoryBus = memoryBus;
            m_PathToRacetracksConverter = pathToRacetracksConverter;

            memoryBus.SubscribeAsync <ColonyBestTrailMessage>(GetType().ToString(),
                                                              ColonyBestTrailHandler);

            memoryBus.SubscribeAsync <ColonyLineResponseMessage>(GetType().ToString(),
                                                                 ColonyLineResponsedHandler);
        }

        public IEnumerable <IPath> Paths
        {
            get
            {
                return m_PathToRacetracksConverter.Paths;
            }
        }

        internal void ColonyLineResponsedHandler(ColonyLineResponseMessage message)
        {
            Update(new int[0]);
        }

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            Update(message.Trail);
        }

        internal void Update(IEnumerable <int> trail)
        {
            m_PathToRacetracksConverter.Path = trail;
            m_PathToRacetracksConverter.Convert();

            m_MemoryBus.Publish(new RacetrackModelChangedMessage());
        }
    }
}