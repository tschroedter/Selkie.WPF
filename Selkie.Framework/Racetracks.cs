using Selkie.Framework.Interfaces;

namespace Selkie.Framework
{
    public class Racetracks : IRacetracks
    {
        public static Racetracks Unknown = new Racetracks(true);
        private IPath[][] m_ForwardToForward = new IPath[0][];
        private IPath[][] m_ForwardToReverse = new IPath[0][];
        private IPath[][] m_ReverseToForward = new IPath[0][];
        private IPath[][] m_ReverseToReverse = new IPath[0][];

        public Racetracks()
        {
        }

        private Racetracks(bool isUnknown)
        {
            IsUnknown = isUnknown;
        }

        public bool IsUnknown { get; private set; }

        public IPath[][] ForwardToForward
        {
            get
            {
                return m_ForwardToForward;
            }
            set
            {
                m_ForwardToForward = value;
            }
        }

        public IPath[][] ForwardToReverse
        {
            get
            {
                return m_ForwardToReverse;
            }
            set
            {
                m_ForwardToReverse = value;
            }
        }

        public IPath[][] ReverseToForward
        {
            get
            {
                return m_ReverseToForward;
            }
            set
            {
                m_ReverseToForward = value;
            }
        }

        public IPath[][] ReverseToReverse
        {
            get
            {
                return m_ReverseToReverse;
            }
            set
            {
                m_ReverseToReverse = value;
            }
        }
    }
}