using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Framework.Aco;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ColonyParametersTests
    {
        [Test]
        public void Constructor_AppliesDefaultForCostMatrix_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new ColonyParameters();

            // Assert
            Assert.NotNull(sut.CostMatrix);
        }

        [Test]
        public void Constructor_AppliesDefaultForCostPerFeature_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new ColonyParameters();

            // Assert
            Assert.NotNull(sut.CostPerFeature);
        }
    }
}