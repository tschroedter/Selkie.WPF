using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Messages;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.Aco.XUnit
{
    [ExcludeFromCodeCoverage]
    public class CostMatrixCalculationManagerTests
    {
        private const double Tolerance = 0.1;

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsLines_WhenCalled(
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            var expected = new[]
                           {
                               Substitute.For <ILine>()
                           };

            manager.Lines.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected,
                         sut.Lines);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsCostPerLine_WhenCalled(
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            var expected = new[]
                           {
                               1,
                               2
                           };

            manager.CostPerLine.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected,
                         sut.CostPerLine);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SendsLinesSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            bus.Received().PublishAsync(Arg.Any <LinesSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsCalculating_RetrunsFalse_ByDefault(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.False(sut.IsCalculating);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetIsCalculating_WhenCalled(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            Assert.True(sut.IsCalculating);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedColonyLinesChangedMessage_RetrunsFalse_ByDefault(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.False(sut.IsReceivedColonyLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedColonyLinesChangedMessage_SetIsCalculating_WhenCalled(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedColonyLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedColonyRacetrackSettingsChangedMessage_RetrunsFalse_ByDefault(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.False(sut.IsReceivedColonyRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedColonyRacetrackSettingsChangedMessage_SetIsCalculating_WhenCalled(
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedColonyRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Matrix_ReturnsConverterMatrix_WhenCalled(
            [NotNull, Frozen] IDoubleArrayToIntegerArrayConverter converter,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            var expected = new[]
                           {
                               new[]
                               {
                                   1,
                                   2
                               }
                           };

            converter.IntegerMatrix.Returns(expected);

            // Act
            int[][] actual = sut.Matrix;

            // Assert
            Assert.Equal(expected,
                         actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDtos_ForLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            var lineOne = new Line(0,
                                   1.0,
                                   2.0,
                                   3.0,
                                   4.0);
            var lineTwo = new Line(1,
                                   5.0,
                                   6.0,
                                   7.0,
                                   8.0);

            // Act
            IEnumerable <LineDto> actual = sut.CreateLineDtos(new[]
                                                              {
                                                                  lineOne,
                                                                  lineTwo
                                                              });

            // Assert
            Assert.True(actual.Count() == 2);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDto_ForLine(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            var line = new Line(0,
                                1.0,
                                2.0,
                                3.0,
                                4.0);

            // Act
            LineDto actual = sut.CreateLineDtos(new[]
                                                {
                                                    line
                                                }).First();

            // Assert
            Assert.True(actual.Id == line.Id,
                        "Id");
            Assert.True(actual.IsUnknown == line.IsUnknown,
                        "IsUnknown");
            Assert.True(actual.RunDirection == line.RunDirection.ToString(),
                        "RunDirection");
            Assert.True(Math.Abs(actual.X1 - line.X1) < Tolerance,
                        "X1");
            Assert.True(Math.Abs(actual.Y1 - line.Y1) < Tolerance,
                        "Y1");
            Assert.True(Math.Abs(actual.X2 - line.X2) < Tolerance,
                        "X2");
            Assert.True(Math.Abs(actual.Y2 - line.Y2) < Tolerance,
                        "Y2");
        }

        [Theory]
        [AutoNSubstituteData]
        public void LinesChangedHandler_DoesNotSendsMessage_WhenIsCalculatingIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] LinesChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.LinesChangedHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void LinesChangedHandler_DoesNotSendsMessage_WhenIsReceivedAndExpectedLineCountDoNotMatch(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] LinesChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(new ILine[0]);
            message.LineDtos = new[]
                               {
                                   new LineDto()
                               };
            sut.Calculate();

            // Act
            sut.LinesChangedHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void LinesChangedHandler_LogsMessage_WhenIsReceivedAndExpectedLineCountDoNotMatch(
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] LinesChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(new ILine[0]);
            message.LineDtos = new[]
                               {
                                   new LineDto()
                               };
            sut.Calculate();

            // Act
            sut.LinesChangedHandler(message);

            // Assert
            logger.Received().Info(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void LinesChangedHandler_SendsMessage_WhenIsCalculatingIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] LinesChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            sut.Calculate();

            // Act
            sut.LinesChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void LinesChangedHandler_SetsIsReceivedFlag_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] LinesChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            sut.Calculate();

            // Act
            sut.LinesChangedHandler(message);

            // Assert
            Assert.True(sut.IsReceivedColonyLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void RacetrackSettingsChangedHandler_DoesNotSendsMessage_WhenIsCalculatingIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] RacetrackSettingsChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.RacetrackSettingsChangedHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void RacetrackSettingsChangedHandler_SendsMessage_WhenIsCalculatingIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] RacetrackSettingsChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            sut.Calculate();

            // Act
            sut.RacetrackSettingsChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void RacetrackSettingsChangedHandler_SetsIsReceivedFlag_WhenCalled(
            [NotNull] RacetrackSettingsChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            sut.Calculate();

            // Act
            sut.RacetrackSettingsChangedHandler(message);

            // Assert
            Assert.True(sut.IsReceivedColonyRacetrackSettingsChangedMessage);
        }


        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_SetsIsReceivedFlag_WhenCalled(
            [NotNull] CostMatrixChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            sut.Calculate();

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            Assert.False(sut.IsCalculating);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_LogsWarning_WhenMatrixIsNull(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            message.Matrix = null;
            sut.Calculate();

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] IDoubleArrayToIntegerArrayConverter converter,
            [NotNull] CostMatrixChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            converter.IntegerMatrix.Returns(new int[0][]);
            manager.CostPerLine.Returns(new int[0]);

            sut.Calculate();

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <CostMatrixCalculatedMessage>(x => x.Matrix == converter.IntegerMatrix &&
                                                                                  x.CostPerLine.Length ==
                                                                                  manager.CostPerLine.Count()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_DoesNotSendsMessage_WhenIsCalculatingIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] IDoubleArrayToIntegerArrayConverter converter,
            [NotNull] CostMatrixChangedMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            converter.IntegerMatrix.Returns(new int[0][]);
            manager.CostPerLine.Returns(new int[0]);

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculatedMessage>());
        }
    }
}