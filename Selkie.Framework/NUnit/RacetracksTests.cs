using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;

namespace Selkie.Framework.NUnit
{
    [TestFixture]
    internal sealed class RacetracksTests
    {
        [SetUp]
        public void Setup()
        {
            m_ForwardToForward = new[]
                                 {
                                     new[]
                                     {
                                         Substitute.For <IPath>()
                                     }
                                 };

            m_ForwardToReverse = new[]
                                 {
                                     new[]
                                     {
                                         Substitute.For <IPath>()
                                     }
                                 };
            m_ReverseToForward = new[]
                                 {
                                     new[]
                                     {
                                         Substitute.For <IPath>()
                                     }
                                 };
            m_ReverseToReverse = new[]
                                 {
                                     new[]
                                     {
                                         Substitute.For <IPath>()
                                     }
                                 };

            m_Sut = new Racetracks
                    {
                        ForwardToForward = m_ForwardToForward,
                        ForwardToReverse = m_ForwardToReverse,
                        ReverseToForward = m_ReverseToForward,
                        ReverseToReverse = m_ReverseToReverse
                    };
        }

        private Racetracks m_Sut;
        private IPath[][] m_ReverseToReverse;
        private IPath[][] m_ForwardToForward;
        private IPath[][] m_ForwardToReverse;
        private IPath[][] m_ReverseToForward;

        [Test]
        public void ForwardToForward_ReturnsDefault()
        {
            Assert.AreEqual(m_ForwardToForward,
                            m_Sut.ForwardToForward);
        }

        [Test]
        public void ForwardToReverse_ReturnsDefault()
        {
            Assert.AreEqual(m_ForwardToReverse,
                            m_Sut.ForwardToReverse);
        }

        [Test]
        public void ReverseToForward_ReturnsDefault()
        {
            Assert.AreEqual(m_ReverseToForward,
                            m_Sut.ReverseToForward);
        }

        [Test]
        public void ReverseToReverse_ReturnsDefault()
        {
            Assert.AreEqual(m_ReverseToReverse,
                            m_Sut.ReverseToReverse);
        }
    }
}