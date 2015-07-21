using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.Framework
{
    public class RacetracksSource : IRacetracks // todo rename interface
    {
        public static IRacetracks Unknown = new RacetracksSource(new IPath[0][],
                                                                 new IPath[0][],
                                                                 new IPath[0][],
                                                                 new IPath[0][]);

        private readonly IPath[][] m_ForwardToForward;
        private readonly IPath[][] m_ForwardToReverse;
        private readonly IPath[][] m_ReverseToForward;
        private readonly IPath[][] m_ReverseToReverse;

        public RacetracksSource([NotNull] IPath[][] forwardToForward,
                                [NotNull] IPath[][] forwardToReverse,
                                [NotNull] IPath[][] reverseToForward,
                                [NotNull] IPath[][] reverseToReverse)
        {
            m_ForwardToForward = forwardToForward;
            m_ForwardToReverse = forwardToReverse;
            m_ReverseToForward = reverseToForward;
            m_ReverseToReverse = reverseToReverse;
        }

        public IPath[][] ForwardToForward
        {
            get
            {
                return m_ForwardToForward;
            }
        }

        public IPath[][] ForwardToReverse
        {
            get
            {
                return m_ForwardToReverse;
            }
        }

        public IPath[][] ReverseToForward
        {
            get
            {
                return m_ReverseToForward;
            }
        }

        public IPath[][] ReverseToReverse
        {
            get
            {
                return m_ReverseToReverse;
            }
        }
    }
}