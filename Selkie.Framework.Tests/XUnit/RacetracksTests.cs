using JetBrains.Annotations;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class RacetracksTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void IsUnknown_ReturnsFalse_ForKnown([NotNull] Racetracks sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.False(sut.IsUnknown);
        }

        [Fact]
        public void IsUnknown_ReturnsTrue_ForUnknown()
        {
            // Arrange
            Racetracks sut = Racetracks.Unknown;

            // Act
            // Assert
            Assert.True(sut.IsUnknown);
        }
    }
}