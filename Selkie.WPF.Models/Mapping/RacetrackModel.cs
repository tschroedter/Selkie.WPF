using System.Collections.Generic;
using Castle.Core.Logging;
using EasyNetQ;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class RacetrackModel : IRacetrackModel
    {
        private readonly IBus m_Bus;
        private readonly IPathToRacetracksConverter m_PathToRacetracksConverter;

        public RacetrackModel(ILogger logger,
                              IBus bus,
                              IPathToRacetracksConverter pathToRacetracksConverter)
        {
            m_Bus = bus;
            m_PathToRacetracksConverter = pathToRacetracksConverter;

            bus.SubscribeHandlerAsync <ColonyBestTrailMessage>(logger,
                                                               GetType().ToString(),
                                                               ColonyBestTrailHandler);

            bus.SubscribeHandlerAsync <ColonyLinesChangedMessage>(logger,
                                                                  GetType().ToString(),
                                                                  ColonyLinesChangedHandler);
        }

        public IEnumerable <IPath> Paths
        {
            get
            {
                return m_PathToRacetracksConverter.Paths;
            }
        }

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
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

            m_Bus.Publish(new RacetrackModelChangedMessage());
        }
    }
}