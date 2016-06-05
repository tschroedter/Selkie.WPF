using JetBrains.Annotations;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    [ProjectComponent(Lifestyle.Transient)]
    public class NodeModelCreator : INodeModelCreator
    {
        public NodeModelCreator([NotNull] INodeIdHelper nodeIdHelper)
        {
            m_NodeIdHelper = nodeIdHelper;
        }

        private readonly INodeIdHelper m_NodeIdHelper;

        public INodeModel CreateNodeModel(int lineId,
                                          int nodeId)
        {
            INodeModel nodeModel = NodeModel.Unknown;

            ILine line = m_NodeIdHelper.GetLine(lineId);

            if ( line == null )
            {
                return nodeModel;
            }

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

            return nodeModel;
        }

        public INodeIdHelper Helper
        {
            get
            {
                return m_NodeIdHelper;
            }
        }
    }
}