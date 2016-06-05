using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public abstract class BaseNodeModel
    {
        protected BaseNodeModel([NotNull] ISelkieInMemoryBus bus,
                                [NotNull] INodeModelCreator nodeModelCreator)
        {
            m_Bus = bus;
            m_NodeModelCreator = nodeModelCreator;

            m_Bus.SubscribeAsync <ColonyBestTrailMessage>(GetType().ToString(),
                                                          ColonyBestTrailHandler);

            m_Bus.SubscribeAsync <ColonyLineResponseMessage>(GetType().ToString(),
                                                             ColonyLineResponsedHandler);
        }

        protected ISelkieInMemoryBus Bus
        {
            get
            {
                return m_Bus;
            }
        }

        public INodeModel Node
        {
            get
            {
                return m_NodeModel;
            }
        }

        public INodeIdHelper Helper
        {
            get
            {
                return m_NodeModelCreator.Helper;
            }
        }

        private readonly ISelkieInMemoryBus m_Bus;
        private readonly INodeModelCreator m_NodeModelCreator;
        private INodeModel m_NodeModel = NodeModel.Unknown;

        public abstract int DetermineNodeId(IEnumerable <int> trail);
        public abstract void SendMessage();

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            Update(message.Trail);
        }

        internal void ColonyLineResponsedHandler(ColonyLineResponseMessage message)
        {
            Update(new int[0]);
        }

        internal void Update(IEnumerable <int> trail)
        {
            IEnumerable <int> enumerable = trail as int[] ?? trail.ToArray();

            if ( !enumerable.Any() )
            {
                m_NodeModel = NodeModel.Unknown;
            }
            else
            {
                int nodeId = DetermineNodeId(enumerable);
                int lineId = Helper.NodeToLine(nodeId);

                m_NodeModel = m_NodeModelCreator.CreateNodeModel(lineId,
                                                                 nodeId);
            }

            SendMessage();
        }
    }
}