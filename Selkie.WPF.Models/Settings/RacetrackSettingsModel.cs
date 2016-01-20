using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Settings
{
    public class RacetrackSettingsModel : IRacetrackSettingsModel
    {
        internal static readonly double DefaultTurnRadius = 30.0;
        private readonly ISelkieBus m_Bus;
        private readonly IRacetrackSettingsSourceManager m_Manager;
        private readonly ISelkieInMemoryBus m_MemoryBus;

        public RacetrackSettingsModel([NotNull] ISelkieBus bus,
                                      [NotNull] ISelkieInMemoryBus memoryBus,
                                      [NotNull] IRacetrackSettingsSourceManager manager)
        {
            m_Bus = bus;
            m_MemoryBus = memoryBus;
            m_Manager = manager;

            string subscriptionId = GetType().ToString();
            memoryBus.SubscribeAsync <RacetrackSettingsSetMessage>(subscriptionId,
                                                                   RacetrackSettingsSetHandler);

            memoryBus.SubscribeAsync <RacetrackSettingsRequestMessage>(subscriptionId,
                                                                       RacetrackSettingsRequestHandler);

            m_Bus.SubscribeAsync <ColonyCostMatrixChangedMessage>(subscriptionId,
                                                                  ColonyCostMatrixChangedHandler);
        }

        public double TurnRadius
        {
            get
            {
                return m_Manager.Source.TurnRadius;
            }
        }

        public bool IsPortTurnAllowed
        {
            get
            {
                return m_Manager.Source.IsPortTurnAllowed;
            }
        }

        public bool IsStarboardTurnAllowed
        {
            get
            {
                return m_Manager.Source.IsStarboardTurnAllowed;
            }
        }

        internal void ColonyCostMatrixChangedHandler(ColonyCostMatrixChangedMessage message)
        {
            IRacetrackSettingsSource source = m_Manager.Source;

            var reply = new RacetrackSettingsChangedMessage
                        {
                            TurnRadius = source.TurnRadius,
                            IsPortTurnAllowed = source.IsPortTurnAllowed,
                            IsStarboardTurnAllowed = source.IsStarboardTurnAllowed
                        };

            m_MemoryBus.PublishAsync(reply);
        }

        internal void RacetrackSettingsRequestHandler(RacetrackSettingsRequestMessage message)
        {
            m_Bus.PublishAsync(new ColonyRacetrackSettingsRequestMessage());
        }

        internal void RacetrackSettingsSetHandler(RacetrackSettingsSetMessage message)
        {
            m_Bus.PublishAsync(new ColonyRacetrackSettingsSetMessage
                               {
                                   TurnRadius = message.TurnRadius,
                                   IsPortTurnAllowed = message.IsPortTurnAllowed,
                                   IsStarboardTurnAllowed = message.IsStarboardTurnAllowed
                               });
        }
    }
}