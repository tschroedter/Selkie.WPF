using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Models.Mapping
{
    public abstract class BaseNodeModel
    {
        private readonly ISelkieBus m_Bus;
        private readonly ISelkieInMemoryBus m_MemoryBus;
        private readonly INodeIdHelper m_NodeIdHelper;
        private INodeModel m_NodeModel = NodeModel.Unknown;

        protected BaseNodeModel([NotNull] ISelkieBus bus,
                                [NotNull] ISelkieInMemoryBus memoryBus,
                                [NotNull] INodeIdHelper nodeIdHelper)
        {
            m_Bus = bus;
            m_MemoryBus = memoryBus;
            m_NodeIdHelper = nodeIdHelper;

            bus.SubscribeAsync <ColonyBestTrailMessage>(GetType().ToString(),
                                                        ColonyBestTrailHandler);

            bus.SubscribeAsync <ColonyLinesChangedMessage>(GetType().ToString(),
                                                           ColonyLinesChangedHandler);
        }

        protected ISelkieBus Bus
        {
            get
            {
                return m_Bus;
            }
        }

        protected ISelkieInMemoryBus MemoryBus
        {
            get
            {
                return m_MemoryBus;
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
                return m_NodeIdHelper;
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
            IEnumerable <int> enumerable = trail as int[] ?? trail.ToArray();

            if ( !enumerable.Any() )
            {
                m_NodeModel = NodeModel.Unknown;
            }
            else
            {
                int nodeId = DetermineNodeId(enumerable);
                int lineId = m_NodeIdHelper.NodeToLine(nodeId);

                m_NodeModel = CreateNodeModel(lineId,
                                              nodeId);
            }

            SendMessage();
        }

        public abstract int DetermineNodeId(IEnumerable <int> trail);
        public abstract void SendMessage();

        internal INodeModel CreateNodeModel(int lineId,
                                            int nodeId)
        {
            INodeModel nodeModel = NodeModel.Unknown;

            ILine line = m_NodeIdHelper.GetLine(lineId);

            if ( line != null )
            {
                bool isForwardNode = m_NodeIdHelper.IsForwardNode(nodeId);

                double x = isForwardNode
                               ? line.X1
                               : line.X2;
                double y = isForwardNode
                               ? line.Y1
                               : line.Y2;
                Angle angle = isForwardNode
                                  ? line.AngleToXAxis
                                  : line.AngleToXAxis + Angle.For180Degrees;

                nodeModel = new NodeModel(nodeId,
                                          x,
                                          y,
                                          angle);
            }

            return nodeModel;
        }
    }
}