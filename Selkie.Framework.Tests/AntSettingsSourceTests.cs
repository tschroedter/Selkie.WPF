using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Selkie.Framework.Tests.XUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class AntSettingsSourceTests
    {
        private const bool DefaultIsFixedStartNode = false;
        private const int DefaultFixedStartNode = 1;

        [Test]
        public void Constructor_SetsFixedStartNode_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsSource(DefaultIsFixedStartNode,
                                            DefaultFixedStartNode);

            // Assert
            Assert.AreEqual(DefaultFixedStartNode,
                            sut.FixedStartNode);
        }

        [Test]
        public void Constructor_SetsIsFixedStartNode_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsSource(DefaultIsFixedStartNode,
                                            DefaultFixedStartNode);

            // Assert
            Assert.AreEqual(DefaultIsFixedStartNode,
                            sut.IsFixedStartNode);
        }
    }
}