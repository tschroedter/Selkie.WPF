using System;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    [ProjectComponent(Lifestyle.Transient)]
    public class NodeIndexHelper : INodeIndexHelper
    {
        public NodeIndexHelper([NotNull] ILinesSourceManager linesSourceManager,
                               [NotNull] INodeIndexToLineConverter nodeIndexToLineConverter)
        {
            m_LinesSourceManager = linesSourceManager;
            m_NodeIndexToLineConverter = nodeIndexToLineConverter;
        }

        private const int ForwardAndReverse = 2;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly INodeIndexToLineConverter m_NodeIndexToLineConverter;

        public ILine NodeIndexToLine(int index)
        {
            m_NodeIndexToLineConverter.NodeIndex = index;
            m_NodeIndexToLineConverter.Convert();
            ILine line = m_NodeIndexToLineConverter.Line;

            return line;
        }

        public Constants.LineDirection NodeIndexToDirection(int index)
        {
            if ( !IsValidIndex(index) )
            {
                throw new ArgumentException("Unknown index '{0}'!".Inject(index));
            }

            return index % 2 == 0
                       ? Constants.LineDirection.Forward
                       : Constants.LineDirection.Reverse;
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < m_LinesSourceManager.Lines.Count() * ForwardAndReverse;
        }
    }
}