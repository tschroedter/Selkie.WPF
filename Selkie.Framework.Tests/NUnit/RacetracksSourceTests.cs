using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;

namespace Selkie.Framework.Tests.NUnit
{
    [TestFixture]
    internal sealed class RacetracksSourceTests
    {
        [SetUp]
        public void Setup()
        {
            m_ForwardToForward = CreatePathArray();
            m_ForwardToReverse = CreatePathArray();
            m_ReverseToForward = CreatePathArray();
            m_ReverseToReverse = CreatePathArray();

            m_Sut = new RacetracksSource(m_ForwardToForward,
                                         m_ForwardToReverse,
                                         m_ReverseToForward,
                                         m_ReverseToReverse);
        }

        private IPath[][] m_ForwardToForward;
        private IPath[][] m_ForwardToReverse;
        private IPath[][] m_ReverseToForward;
        private IPath[][] m_ReverseToReverse;
        private RacetracksSource m_Sut;

        private static IPath[][] CreatePathArray()
        {
            return new[]
                   {
                       new[]
                       {
                           Substitute.For <IPath>()
                       },
                       new[]
                       {
                           Substitute.For <IPath>()
                       }
                   };
        }

        [Test]
        public void ForwardToForward_ReturnsValue()
        {
            Assert.AreEqual(m_ForwardToForward,
                            m_Sut.ForwardToForward);
        }

        [Test]
        public void ForwardToReverse_ReturnsValue()
        {
            Assert.AreEqual(m_ForwardToReverse,
                            m_Sut.ForwardToReverse);
        }

        [Test]
        public void IsUnknown_ReturnsFalse()
        {
            Assert.False(m_Sut.IsUnknown);
        }

        [Test]
        public void IsUnknown_ReturnsTrue_ForUnknown()
        {
            IRacetracks sut = RacetracksSource.Unknown;

            Assert.True(sut.IsUnknown);
        }

        [Test]
        public void ReverseToForward_ReturnsValue()
        {
            Assert.AreEqual(m_ReverseToForward,
                            m_Sut.ReverseToForward);
        }

        [Test]
        public void ReverseToReverse_ReturnsValue()
        {
            Assert.AreEqual(m_ReverseToReverse,
                            m_Sut.ReverseToReverse);
        }
    }
}