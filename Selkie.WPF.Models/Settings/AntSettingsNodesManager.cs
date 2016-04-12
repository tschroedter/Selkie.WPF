using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Settings
{
    [ProjectComponent(Lifestyle.Transient)]
    public class AntSettingsNodesManager : IAntSettingsNodesManager
    {
        private readonly IAntSettingsNodeFactory m_AntSettingsNodeFactory;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ILinesToNodesConverter m_LinesToNodesConverter;

        public AntSettingsNodesManager([NotNull] IAntSettingsNodeFactory antSettingsNodeFactory,
                                       [NotNull] ILinesSourceManager linesSourceManager,
                                       [NotNull] ILinesToNodesConverter linesToNodesConverter)
        {
            m_AntSettingsNodeFactory = antSettingsNodeFactory;
            m_LinesSourceManager = linesSourceManager;
            m_LinesToNodesConverter = linesToNodesConverter;

            Nodes = new IAntSettingsNode[0];
        }

        [NotNull]
        public IEnumerable <IAntSettingsNode> Nodes { get; private set; }

        public void CreateNodesForCurrentLines()
        {
            m_LinesToNodesConverter.Convert(m_AntSettingsNodeFactory,
                                            m_LinesSourceManager.Lines);


            IEnumerable <IAntSettingsNode> oldNodes = Nodes;
            Nodes = m_LinesToNodesConverter.Nodes;
            ReleaseNodes(oldNodes);
        }

        internal void ReleaseNodes([NotNull] IEnumerable <IAntSettingsNode> nodes)
        {
            foreach ( IAntSettingsNode antSettingsNode in nodes )
            {
                m_AntSettingsNodeFactory.Release(antSettingsNode);
            }
        }
    }
}