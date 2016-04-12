using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Selkie.Framework.Aco;
using Selkie.Framework.Interfaces;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.Aco.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class ColonyParametersValidatorTests
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
            colonyParameters.CostPerLine = CreateValidCostPerLine();
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = DefaultFixedStartNode;

            // Act
            // Assert
            Assert.DoesNotThrow(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenCostPerLineIsEmpty(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = -1;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerLine = CreateValidCostPerLine();
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
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
            colonyParameters.CostPerLine = new int[0];
            colonyParameters.IsFixedStartNode = DefaultIsFixedStartNode;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Validate_Throws_WhenCostMatrixAndCostPerLineDoNotMatch(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ColonyParametersValidator sut)
        {
            // Arrange
            const int invalidFixedStartNode = 3;
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerLine = new[]
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
            colonyParameters.CostPerLine = CreateValidCostPerLine();
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
            colonyParameters.CostPerLine = CreateValidCostPerLine();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = invalidFixedStartNode;

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.Validate(colonyParameters));
        }

        private static int[] CreateValidCostPerLine()
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