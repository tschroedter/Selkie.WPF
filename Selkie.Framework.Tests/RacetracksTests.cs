using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.NUnit.Extensions;

namespace Selkie.Framework.Tests
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

        [Theory]
        [AutoNSubstituteData]
        public void IsUnknown_ReturnsFalse_ForKnown([NotNull] Racetracks sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.False(sut.IsUnknown);
        }

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
        public void IsUnknown_ReturnsTrue_ForUnknown()
        {
            // Arrange
            Racetracks sut = Racetracks.Unknown;

            // Act
            // Assert
            Assert.True(sut.IsUnknown);
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