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
        internal static readonly double DefaultRadius = 30.0;
        private readonly IBus m_Bus;
        private readonly IRacetrackSettingsSourceFactory m_Factory;

        public RacetrackSettingsSourceManager([NotNull] ILogger logger,
                                              [NotNull] IBus bus,
                                              [NotNull] IRacetrackSettingsSourceFactory factory)
        {
            m_Bus = bus;
            m_Factory = factory;

            Source = m_Factory.Create(DefaultRadius,
                                      true,
                                      true);

            string subscriptionId = GetType().FullName;
            m_Bus.SubscribeHandlerAsync <ColonyRacetrackSettingsSetMessage>(logger,
                                                                            subscriptionId,
                                                                            ColonyRacetrackSettingsSetHandler);

            m_Bus.SubscribeHandlerAsync <ColonyRacetrackSettingsRequestMessage>(logger,
                                                                                subscriptionId,
                                                                                ColonyRacetrackSettingsRequestHandler);
        }

        public IRacetrackSettingsSource Source { get; private set; }

        internal void ColonyRacetrackSettingsRequestHandler(ColonyRacetrackSettingsRequestMessage message)
        {
            m_Bus.PublishAsync(new ColonyRacetrackSettingsChangedMessage());
        }

        internal void ColonyRacetrackSettingsSetHandler(ColonyRacetrackSettingsSetMessage message)
        {
            m_Factory.Release(Source);

            Source = m_Factory.Create(message.TurnRadius,
                                      message.IsPortTurnAllowed,
                                      message.IsStarboardTurnAllowed);

            m_Bus.PublishAsync(new ColonyRacetrackSettingsChangedMessage());
        }
    }
}