using System.Linq;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class NodeIndexToLineConverter : INodeIndexToLineConverter
    {
        private readonly ILinesSourceManager m_LinesSourceManager;
        private ILine m_Line = Geometry.Shapes.Line.Unknown;
        private int m_NodeIndex = int.MinValue;

        public NodeIndexToLineConverter(ILinesSourceManager linesSourceManager)
        {
            m_LinesSourceManager = linesSourceManager;
        }

        public int NodeIndex
        {
            get
            {
                return m_NodeIndex;
            }
            set
            {
                m_NodeIndex = value;
            }
        }

        public ILine Line
        {
            get
            {
                return m_Line;
            }
        }

        public void Convert()
        {
            m_Line = GetLineByNodeIndex(m_NodeIndex);
        }

        internal ILine GetLineByNodeIndex(int index)
        {
            ILine line;

            int lineIndex = index / 2;

            ILine[] lines = m_LinesSourceManager.Lines.ToArray();

            if ( lineIndex >= 0 &&
                 lineIndex < lines.Count() )
            {
                line = lines.ElementAt(lineIndex);
            }
            else
            {
                line = Geometry.Shapes.Line.Unknown;
            }

            return line;
        }
    }
}