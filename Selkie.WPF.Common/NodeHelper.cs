using System.Collections.Generic;
using System.Linq;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Common
{
    [ProjectComponent(Lifestyle.Transient)]
    public sealed class NodeIdHelper : INodeIdHelper
    {
        private readonly ILinesSourceManager m_LinesSourceManager;

        public NodeIdHelper(ILinesSourceManager linesSourceManager)
        {
            m_LinesSourceManager = linesSourceManager;
        }

        public int NodeToLine(int node)
        {
            if ( node == 0 ||
                 node == 1 )
            {
                return 0;
            }

            return node / 2;
        }

        public bool IsForwardNode(int node)
        {
            if ( node == 0 )
            {
                return true;
            }

            if ( node == 1 )
            {
                return false;
            }

            int condition = node % 2;

            return condition == 0;
        }

        public ILine GetLine(int lineId)
        {
            IEnumerable <ILine> lines = m_LinesSourceManager.Lines;

            ILine line = lines.FirstOrDefault(x => x.Id == lineId);

            return line;
        }

        public int Reverse(int nodeId)
        {
            bool isEndPoint = ( nodeId % 2 == 1 );

            if ( isEndPoint )
            {
                return nodeId - 1;
            }

            return nodeId + 1;
        }
    }
}