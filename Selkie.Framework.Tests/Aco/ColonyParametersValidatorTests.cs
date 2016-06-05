using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NUnit.Framework;
using Selkie.Framework.Aco;
using Selkie.Framework.Interfaces;
using Selkie.NUnit.Extensions;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ColonyParametersValidatorTests
    {
        private const bool DefaultIsFixedStartNode = false;
        private const int DefaultFixedStartNode = 1;

        [Theory]
        [AutoNSubstituteData]
        public void Validate_DoesNotThrow_WhenValid(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = DefaultFixedStartNode;

            // Act
            // Assert
            Assert.DoesNotThrow(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenCostMatrixAndCostPerFeatureDoNotMatch(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = 3;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = new[]
                                              {
                                                  1
                                              };
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenCostMatrixIsEmpty(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = 3;
            colonyParameters.CostMatrix = new int[0][];
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenCostPerFeatureIsEmpty(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = -1;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }


        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenFixedStartNodeIsGreaterLineCount(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = 3;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenFixedStartNodeIsNegative(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = -1;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = new int[0];
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        private static int[] CreateValidCostPerFeature()
        {
            var costPerLine = new[]
                              {
                                  1,
                                  2
                              };
            return costPerLine;
        }

        private static int[][] CreateValidMatrix()
        {
            var matrix = new[]
                         {
                             new[]
                             {
                                 1,
                                 1,
                                 1,
                                 1
                             },
                             new[]
                             {
                                 2,
                                 2,
                                 2,
                                 2
                             }
                         };
            return matrix;
        }
    }
}