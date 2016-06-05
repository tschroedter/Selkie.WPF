using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.Framework.Aco;
using Selkie.NUnit.Extensions;
using Selkie.Windsor;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class AcoProxyLoggerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Error_CallsLogger_WhenCalled([NotNull, Frozen] ISelkieLogger logger,
                                                 [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            // Act
            sut.Error("Hello");

            // Assert
            logger.Received().Error(Arg.Is <string>(x => x == "Hello"));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Info_CallsLogger_WhenCalled([NotNull, Frozen] ISelkieLogger logger,
                                                [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            // Act
            sut.Info("Hello");

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "Hello"));
        }

        [Theory]
        [AutoNSubstituteData]
        public void LogCostMatrix_CallsLogger_ForEmptyMatrix([NotNull, Frozen] ISelkieLogger logger,
                                                             [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            var matrix = new int[0][];

            // Act
            sut.LogCostMatrix(matrix);

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "CostMatrix: { }"));
        }

        [Theory]
        [AutoNSubstituteData]
        public void LogCostMatrix_CallsLogger_ForMatrix([NotNull, Frozen] ISelkieLogger logger,
                                                        [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            int[][] matrix = CreateMatrix();

            // Act
            sut.LogCostMatrix(matrix);

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "CostMatrix\r\n[0] 1,2\r\n[1] 1,2\r\n"));
        }

        [Theory]
        [AutoNSubstituteData]
        public void LogCostPerFeature_CallsLogger_WhenCalled([NotNull, Frozen] ISelkieLogger logger,
                                                             [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            var costPerLine = new[]
                              {
                                  1,
                                  2,
                                  3
                              };

            // Act
            sut.LogCostPerFeature(costPerLine);

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "CostPerFeature: 1,2,3"));
        }

        private static int[][] CreateMatrix()
        {
            var matrix = new[]
                         {
                             new[]
                             {
                                 1,
                                 2
                             },
                             new[]
                             {
                                 1,
                                 2
                             }
                         };
            return matrix;
        }
    }
}