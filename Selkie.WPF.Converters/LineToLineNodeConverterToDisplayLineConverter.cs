using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public sealed class LineToLineNodeConverterToDisplayLineConverter : ILineToLineNodeConverterToDisplayLineConverter
    {
        private readonly IDisplayLineFactory m_DisplayLineFactory;
        private readonly IDisposer m_Disposer;
        private IEnumerable <ILineToLineNodeConverter> m_Converters = new ILineToLineNodeConverter[0];
        private List <IDisplayLine> m_DisplayLines = new List <IDisplayLine>();

        public LineToLineNodeConverterToDisplayLineConverter([NotNull] IDisposer disposer,
                                                             [NotNull] IDisplayLineFactory factory)
        {
            m_DisplayLineFactory = factory;
            m_Disposer = disposer;

            m_Disposer.AddResource(ReleaseDisplayLines);
        }

        public void Dispose()
        {
            m_Disposer.Dispose();
        }

        public IEnumerable <ILineToLineNodeConverter> Converters
        {
            get
            {
                return m_Converters;
            }
            set
            {
                m_Converters = value;
            }
        }

        public IEnumerable <IDisplayLine> DisplayLines
        {
            get
            {
                return m_DisplayLines;
            }
        }

        public void Convert()
        {
            m_DisplayLines = CreateDisplayLines(m_Converters);
        }

        internal void ReleaseDisplayLines()
        {
            foreach ( IDisplayLine displayLine in m_DisplayLines )
            {
                m_DisplayLineFactory.Release(displayLine);
            }

            m_DisplayLines.Clear();
        }

        internal List <IDisplayLine> CreateDisplayLines([NotNull] IEnumerable <ILineToLineNodeConverter> nodes)
        {
            ILineToLineNodeConverter[] nodesArray = nodes.ToArray();

            List <IDisplayLine> displayLines = nodesArray.Length == 1
                                                   ? CreateDisplayLinesSingleNode(nodesArray)
                                                   : CreateDisplayLinesForNodes(nodesArray);

            return displayLines;
        }

        internal List <IDisplayLine> CreateDisplayLinesForNodes(ILineToLineNodeConverter[] nodes)
        {
            var lines = new List <IDisplayLine>();

            for ( var i = 0 ; i < nodes.Length ; i++ )
            {
                ILineToLineNodeConverter node = nodes [ i ];

                IDisplayLine fromLine = m_DisplayLineFactory.Create(node.From,
                                                                    node.FromDirection);
                lines.Add(fromLine);

                if ( i == nodes.Length - 1 )
                {
                    IDisplayLine toLine = m_DisplayLineFactory.Create(node.To,
                                                                      node.ToDirection);
                    lines.Add(toLine);
                }
            }

            return lines;
        }

        internal List <IDisplayLine> CreateDisplayLinesSingleNode(ILineToLineNodeConverter[] nodes)
        {
            var lines = new List <IDisplayLine>();

            ILineToLineNodeConverter onlyNode = nodes [ 0 ];

            IDisplayLine fromLine = m_DisplayLineFactory.Create(onlyNode.From,
                                                                onlyNode.FromDirection);
            IDisplayLine toLine = m_DisplayLineFactory.Create(onlyNode.To,
                                                              onlyNode.ToDirection);

            lines.Add(fromLine);
            lines.Add(toLine);

            return lines;
        }
    }
}