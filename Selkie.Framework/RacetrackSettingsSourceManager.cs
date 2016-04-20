using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class RacetrackSettingsSourceManager : IRacetrackSettingsSourceManager
    {
        internal static readonly double DefaultRadius = 30.0;
        private readonly ISelkieBus m_Bus;
        private readonly IRacetrackSettingsSourceFactory m_Factory;

        public RacetrackSettingsSourceManager([NotNull] ISelkieBus bus,
                                              [NotNull] IRacetrackSettingsSourceFactory factory)
        {
            m_Bus = bus;
            m_Factory = factory;

            Source = m_Factory.Create(DefaultRadius,
                                      DefaultRadius,
                                      true,
                                      true);

            string subscriptionId = GetType().FullName;
            m_Bus.SubscribeAsync <ColonyRacetrackSettingsSetMessage>(subscriptionId,
                                                                     ColonyRacetrackSettingsSetHandler);

            m_Bus.SubscribeAsync <ColonyRacetrackSettingsRequestMessage>(subscriptionId,
                                                                         ColonyRacetrackSettingsRequestHandler);
        }

        public IRacetrackSettingsSource Source { get; private set; }

        internal void ColonyRacetrackSettingsRequestHandler(ColonyRacetrackSettingsRequestMessage message)
        {
            m_Bus.PublishAsync(new ColonyRacetrackSettingsResponseMessage());
        }

        internal void ColonyRacetrackSettingsSetHandler(ColonyRacetrackSettingsSetMessage message)
        {
            m_Factory.Release(Source);

            Source = m_Factory.Create(message.TurnRadiusForPort,
                                      message.TurnRadiusForStarboard,
                                      message.IsPortTurnAllowed,
                                      message.IsStarboardTurnAllowed);

            m_Bus.PublishAsync(new ColonyRacetrackSettingsResponseMessage());
        }
    }
}