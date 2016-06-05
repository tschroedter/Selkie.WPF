using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using Selkie.Framework.Converters;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;
using Selkie.NUnit.Extensions;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    internal sealed class SurveyFeatureToSurveyFeatureDtoConverterTests
    {
        private const double Tolerance = 0.1;

        [Theory]
        [AutoNSubstituteData]
        public void Convert_ReturnsCorrectNumberOfDtos_ForFeatures(
            [NotNull] ISurveyFeature[] features,
            [NotNull] SurveyFeatureToSurveyFeatureDtoConverter sut)
        {
            // Arrange
            sut.Features = features;

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(features.Length,
                            sut.Dtos.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Convert_ReturnsDto_ForFeature(
            [NotNull] ISurveyFeature feature,
            [NotNull] SurveyFeatureToSurveyFeatureDtoConverter sut)
        {
            // Arrange
            sut.Features = new[]
                           {
                               feature
                           };

            // Act
            sut.Convert();

            // Assert
            SurveyFeatureDto actual = sut.Dtos.First();

            AssertFeatureVsDto(feature,
                               actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Dtos_ReturnsNotNull_WhenCalled(
            [NotNull] SurveyFeatureToSurveyFeatureDtoConverter sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Dtos);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Features_ReturnsNotNull_WhenCalled(
            [NotNull] SurveyFeatureToSurveyFeatureDtoConverter sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Features);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Features_Roundtrip_Test(
            [NotNull] ISurveyFeature[] features,
            [NotNull] SurveyFeatureToSurveyFeatureDtoConverter sut)
        {
            // Arrange
            // Act
            sut.Features = features;

            // Assert
            Assert.AreEqual(features,
                            sut.Features);
        }

        // ReSharper disable UnusedParameter.Local
        private static void AssertFeatureVsDto([NotNull] ISurveyFeature feature,
                                               [NotNull] SurveyFeatureDto actual)
            // ReSharper restore UnusedParameter.Local
        {
            Assert.True(actual.Id == feature.Id,
                        "Id");
            Assert.True(actual.IsUnknown == feature.IsUnknown,
                        "IsUnknown");
            Assert.True(actual.RunDirection == feature.RunDirection.ToString(),
                        "RunDirection");
            AssertPointVsPointDto(feature.StartPoint,
                                  actual.StartPoint,
                                  "StartPoint");
            AssertPointVsPointDto(feature.EndPoint,
                                  actual.EndPoint,
                                  "EndPoint");
            Assert.True(
                        Math.Abs(actual.AngleToXAxisAtStartPoint - feature.AngleToXAxisAtStartPoint.Degrees) < Tolerance,
                        "AngleToXAxisAtStartPoint");
            Assert.True(Math.Abs(actual.AngleToXAxisAtEndPoint - feature.AngleToXAxisAtEndPoint.Degrees) < Tolerance,
                        "AngleToXAxisAtEndPoint");
        }

        // ReSharper disable UnusedParameter.Local
        private static void AssertPointVsPointDto([NotNull] Point expected,
                                                  [NotNull] PointDto actual,
                                                  [NotNull] string description)
            // ReSharper restore UnusedParameter.Local
        {
            Assert.True(Math.Abs(actual.X - expected.X) < Tolerance,
                        string.Format(description + " X: Expected = {0} but Actual = {1}",
                                      expected.X,
                                      actual.X));

            Assert.True(Math.Abs(actual.Y - expected.Y) < Tolerance,
                        string.Format(description + " Y: Expected = {0} but Actual = {1}",
                                      expected.Y,
                                      actual.Y));
        }
    }
}