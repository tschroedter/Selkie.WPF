using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class AntSettingsSource : IAntSettingsSource
    {
        private readonly int m_FixedStartNode;
        private readonly bool m_IsFixedStartNode;

        public AntSettingsSource(bool isFixedStartNode,
                                 int fixedStartNode)

        {
            m_IsFixedStartNode = isFixedStartNode;
            m_FixedStartNode = fixedStartNode;
        }

        public bool IsFixedStartNode
        {
            get
            {
                return m_IsFixedStartNode;
            }
        }

        public int FixedStartNode
        {
            get
            {
                return m_FixedStartNode;
            }
        }
    }
}