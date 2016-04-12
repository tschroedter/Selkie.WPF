using Xunit;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class AntSettingsSourceTests
    {
        private const bool DefaultIsFixedStartNode = false;
        private const int DefaultFixedStartNode = 1;

        [Fact]
        public void Constructor_SetsIsFixedStartNode_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsSource(DefaultIsFixedStartNode,
                                            DefaultFixedStartNode);

            // Assert
            Assert.Equal(DefaultIsFixedStartNode,
                         sut.IsFixedStartNode);
        }

        [Fact]
        public void Constructor_SetsFixedStartNode_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsSource(DefaultIsFixedStartNode,
                                            DefaultFixedStartNode);

            // Assert
            Assert.Equal(DefaultFixedStartNode,
                         sut.FixedStartNode);
        }
    }
}