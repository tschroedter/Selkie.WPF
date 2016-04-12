using System.Diagnostics.CodeAnalysis;
using Selkie.Framework.Aco;
using Xunit;

namespace Selkie.Framework.Tests.Aco.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class ColonyParametersTests
    {
        [Fact]
        public void Constructor_AppliesDefaultForCostMatrix_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new ColonyParameters();

            // Assert
            Assert.NotNull(sut.CostMatrix);
        }

        [Fact]
        public void Constructor_AppliesDefaultForCostPerLine_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new ColonyParameters();

            // Assert
            Assert.NotNull(sut.CostPerLine);
        }
    }
}