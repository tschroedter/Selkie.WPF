using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Settings
{
    public class AntSettingsModel : IAntSettingsModel
    {
        private readonly IAntSettingsNodesManager m_AntSettingsNodesManager;
        private readonly ISelkieInMemoryBus m_InMemoryBus;

        public AntSettingsModel([NotNull] ISelkieInMemoryBus inMemoryBus,
                                [NotNull] IAntSettingsNodesManager antSettingsNodesManager)
        {
            m_InMemoryBus = inMemoryBus;
            m_AntSettingsNodesManager = antSettingsNodesManager;

            string subscriptionId = GetType().ToString();

            m_InMemoryBus.SubscribeAsync <ColonyAntSettingsResponseMessage>(subscriptionId,
                                                                            ColonyAntSettingsResponseHandler);
            m_InMemoryBus.SubscribeAsync <ColonyLinesChangedMessage>(subscriptionId,
                                                                     ColonyLinesChangedHandler);
            m_InMemoryBus.SubscribeAsync <AntSettingsModelRequestMessage>(subscriptionId,
                                                                          AntSettingsModelRequestHandler);
            m_InMemoryBus.SubscribeAsync <AntSettingsModelSetMessage>(subscriptionId,
                                                                      AntSettingsModelSetHandler);

            m_InMemoryBus.PublishAsync(new ColonyLinesRequestMessage());
        }

        public bool IsFixedStartNode { get; private set; }
        public int FixedStartNode { get; private set; }

        public IEnumerable <IAntSettingsNode> Nodes
        {
            get
            {
                return m_AntSettingsNodesManager.Nodes;
            }
        }

        internal void ColonyAntSettingsResponseHandler(ColonyAntSettingsResponseMessage message)
        {
            IsFixedStartNode = message.IsFixedStartNode;
            FixedStartNode = message.FixedStartNode;

            SendAntSettingsModelChangedMessage();
        }

        private void SendAntSettingsModelChangedMessage()
        {
            var forward = new AntSettingsModelChangedMessage(IsFixedStartNode,
                                                             FixedStartNode,
                                                             Nodes);

            m_InMemoryBus.PublishAsync(forward);
        }

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            CreateNodesForCurrentLines();
        }

        private void CreateNodesForCurrentLines()
        {
            m_AntSettingsNodesManager.CreateNodesForCurrentLines();

            SendAntSettingsModelChangedMessage();
        }

        internal void AntSettingsModelSetHandler(AntSettingsModelSetMessage message)
        {
            var forward = new ColonyAntSettingsSetMessage
                          {
                              IsFixedStartNode = message.IsFixedStartNode,
                              FixedStartNode = message.FixedStartNode
                          };

            m_InMemoryBus.PublishAsync(forward);
        }

        internal void AntSettingsModelRequestHandler(AntSettingsModelRequestMessage message)
        {
            SendAntSettingsModelChangedMessage();
        }
    }
}