﻿using System.Collections.Generic;
using Castle.Core.Logging;
using EasyNetQ;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public sealed class ShortestPathDirectionModel : IShortestPathDirectionModel
    {
        private readonly IBus m_Bus;
        private readonly INodeIdHelper m_NodeIdHelper;
        private readonly List <INodeModel> m_Nodes = new List <INodeModel>();

        public ShortestPathDirectionModel(ILogger logger,
                                          IBus bus,
                                          INodeIdHelper nodeIdHelper)
        {
            m_Bus = bus;
            m_NodeIdHelper = nodeIdHelper;

            bus.SubscribeHandlerAsync <ColonyBestTrailMessage>(logger,
                                                               GetType().FullName,
                                                               ColonyBestTrailHandler);

            bus.SubscribeHandlerAsync <ColonyLinesChangedMessage>(logger,
                                                                  GetType().ToString(),
                                                                  ColonyLinesChangedHandler);
        }

        public IEnumerable <INodeModel> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            UpdateNodes(new int[0]);
        }

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            Update(message);
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
                int lineId = m_NodeIdHelper.NodeToLine(nodeId);

                INodeModel model = CreateNodeModel(lineId,
                                                   nodeId);

                m_Nodes.Add(model);
            }

            m_Bus.Publish(new ShortestPathDirectionModelChangedMessage());
        }

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