using System.Diagnostics.CodeAnalysis;
using Castle.Core.Logging;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.XUnit.Extensions;
using Xunit.Extensions;

namespace Selkie.Framework.Aco.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class AcoProxyLoggerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Error_CallsLogger_WhenCalled([NotNull, Frozen] ILogger logger,
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
        public void Info_CallsLogger_WhenCalled([NotNull, Frozen] ILogger logger,
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
        public void LogCostPerLine_CallsLogger_WhenCalled([NotNull, Frozen] ILogger logger,
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
            sut.LogCostPerLine(costPerLine);

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "CostPerLine: 1,2,3"));
        }

        [Theory]
        [AutoNSubstituteData]
        public void LogCostMatrix_CallsLogger_ForMatrix([NotNull, Frozen] ILogger logger,
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
        public void LogCostMatrix_CallsLogger_ForEmptyMatrix([NotNull, Frozen] ILogger logger,
                                                             [NotNull] AcoProxyLogger sut)
        {
            // Arrange
            var matrix = new int[0][];

            // Act
            sut.LogCostMatrix(matrix);

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x == "CostMatrix: { }"));
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