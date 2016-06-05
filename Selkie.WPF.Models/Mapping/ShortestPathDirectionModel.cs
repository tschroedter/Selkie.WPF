using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public sealed class ShortestPathDirectionModel : IShortestPathDirectionModel
    {
        public ShortestPathDirectionModel([NotNull] ISelkieInMemoryBus memoryBus,
                                          [NotNull] INodeModelCreator nodeModelCreator)
        {
            m_MemoryBus = memoryBus;
            m_NodeModelCreator = nodeModelCreator;

            memoryBus.SubscribeAsync <ColonyBestTrailMessage>(GetType().FullName,
                                                              ColonyBestTrailHandler);

            memoryBus.SubscribeAsync <ColonyLineResponseMessage>(GetType().ToString(),
                                                                 ColonyLineResponsedHandler);
        }

        public INodeIdHelper Helper
        {
            get
            {
                return m_NodeModelCreator.Helper;
            }
        }

        private readonly ISelkieInMemoryBus m_MemoryBus;
        private readonly INodeModelCreator m_NodeModelCreator;
        private readonly List <INodeModel> m_Nodes = new List <INodeModel>();

        public IEnumerable <INodeModel> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            Update(message);
        }

        internal void ColonyLineResponsedHandler(ColonyLineResponseMessage message)
        {
            UpdateNodes(new int[0]);
        }

        internal void Update(ColonyBestTrailMessage message)
        {
            UpdateNodes(message.Trail);
        }

        internal void UpdateNodes(IEnumerable <int> trail)
        {
            m_Nodes.Clear();

            foreach ( int nodeId in trail )
            {
                int lineId = Helper.NodeToLine(nodeId);

                INodeModel model = m_NodeModelCreator.CreateNodeModel(lineId,
                                                                      nodeId);

                m_Nodes.Add(model);
            }

            m_MemoryBus.Publish(new ShortestPathDirectionModelChangedMessage());
        }
    }
}