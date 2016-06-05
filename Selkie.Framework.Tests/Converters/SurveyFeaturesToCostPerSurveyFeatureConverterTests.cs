using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using Selkie.Framework.Converters;
using Selkie.Geometry.Surveying;
using Selkie.NUnit.Extensions;

namespace Selkie.Framework.Tests.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class SurveyFeaturesToCostPerSurveyFeatureConverterTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Convert_SetsCostPerFeature_ForFeatures(
            [NotNull] ISurveyFeature one,
            [NotNull] ISurveyFeature two,
            [NotNull] SurveyFeaturesToCostPerSurveyFeatureConverter sut)
        {
            // Arrange
            sut.Features = new[]
                           {
                               one,
                               two
                           };

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(( int ) one.Length,
                            sut.CostPerFeature.ElementAt(0));
            Assert.AreEqual(( int ) one.Length,
                            sut.CostPerFeature.ElementAt(1));
            Assert.AreEqual(( int ) two.Length,
                            sut.CostPerFeature.ElementAt(2));
            Assert.AreEqual(( int ) two.Length,
                            sut.CostPerFeature.ElementAt(3));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Convert_SetsCostPerFeatureCorrectLength_ForFeatures(
            [NotNull] ISurveyFeature[] features,
            [NotNull] SurveyFeaturesToCostPerSurveyFeatureConverter sut)
        {
            // Arrange
            int expected = features.Length * 2; // forward + reverse

            sut.Features = features;

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(expected,
                            sut.CostPerFeature.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostPerFeature_ReturnsNotNull_WhenCalled(
            [NotNull] SurveyFeaturesToCostPerSurveyFeatureConverter sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.CostPerFeature);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Features_ReturnsNotNull_WhenCalled(
            [NotNull] SurveyFeaturesToCostPerSurveyFeatureConverter sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Features);
        }
    }
}