using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class RacetrackSettingsSourceManager : IRacetrackSettingsSourceManager
    {
        private readonly IBus m_Bus;
        private IRacetrackSettingsSource m_Source = RacetrackSettingsSource.Default;

        public RacetrackSettingsSourceManager([NotNull] ILogger logger,
                                              [NotNull] IBus bus)
        {
            m_Bus = bus;

            string subscriptionId = GetType().FullName;
            m_Bus.SubscribeHandlerAsync <ColonyRacetrackSettingsSetMessage>(logger,
                                                                            subscriptionId,
                                                                            ColonyRacetrackSettingsSetHandler);

            m_Bus.SubscribeHandlerAsync <ColonyRacetrackSettingsRequestMessage>(logger,
                                                                                subscriptionId,
                                                                                ColonyRacetrackSettingsRequestHandler);
        }

        public IRacetrackSettingsSource Source
        {
            get
            {
                return m_Source;
            }
        }

        internal void ColonyRacetrackSettingsRequestHandler(ColonyRacetrackSettingsRequestMessage message)
        {
            m_Bus.PublishAsync(new ColonyRacetrackSettingsChangedMessage());
        }

        internal void ColonyRacetrackSettingsSetHandler(ColonyRacetrackSettingsSetMessage message)
        {
            // todo replace with factory
            m_Source = new RacetrackSettingsSource(message.TurnRadius,
                                                   message.IsPortTurnAllowed,
                                                   message.IsStarboardTurnAllowed);

            m_Bus.PublishAsync(new ColonyRacetrackSettingsChangedMessage());
        }
    }
}