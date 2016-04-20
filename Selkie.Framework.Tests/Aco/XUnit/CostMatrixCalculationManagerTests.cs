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
            ILine[] expected = CreateLines().ToArray();

            manager.Lines.Returns(expected);
            manager.CostPerLine.Returns(CreateCostPerLine());

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
            int[] expected = CreateCostPerLine();

            manager.Lines.Returns(CreateLines());
            manager.CostPerLine.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected,
                         sut.CostPerLine);
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
        public void CostMatrixResponseHandler_LogsWarning_WhenMatrixIsNull(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            message.Matrix = null;
            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] IDoubleArrayToIntegerArrayConverter converter,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            converter.IntegerMatrix.Returns(CreateIntegerMatrix());
            manager.Lines.Returns(CreateLines());
            manager.CostPerLine.Returns(CreateCostPerLine());
            message.Matrix = CreateDoubleMatrix();

            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <CostMatrixCalculatedMessage>(x => x.Matrix == converter.IntegerMatrix &&
                                                                                  x.CostPerLine.Length ==
                                                                                  manager.CostPerLine.Count()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_DoesNotSendsMessage_WhenIsCalculatingIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] IDoubleArrayToIntegerArrayConverter converter,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(CreateLines());
            manager.CostPerLine.Returns(CreateCostPerLine());
            message.Matrix = CreateDoubleMatrix();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculatedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_DoesNotSendsMessage_WhenMatrixIsEmpty(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(new ILine[0]);
            message.Matrix = new double[0][];

            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculatedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_DoesNotSendsMessage_WhenMatrixIsNull(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(new ILine[0]);
            message.Matrix = null;

            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculatedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_DoesNotSendsMessage_WhenMatrixDoesNotMatchLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(new ILine[0]);
            message.Matrix = new[]
                             {
                                 new[]
                                 {
                                     0.0
                                 }
                             };

            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculatedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixResponseHandler_LogsMessage_WhenMatrixDoesNotMatchLines(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.Lines.Returns(CreateLines());
            manager.CostPerLine.Returns(CreateCostPerLine());

            message.Matrix = new[]
                             {
                                 new[]
                                 {
                                     0.0
                                 }
                             };


            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            logger.Received().Info(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendCostMatrixCalculateMessage_SendsMessage_WhenMatrixDoesNotMatchLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            LineDto[] lineDtos = CreateLineDtos();
            IRacetrackSettingsSource settings = CreateSettings();


            // Act
            sut.SendCostMatrixCalculateMessage(lineDtos,
                                               settings);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <CostMatrixCalculateMessage>(x =>
                                                                 IsValidCostMatrixCalculateMessage(x,
                                                                                                   lineDtos,
                                                                                                   settings)));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_DoesNotSendMessage_ForIsAllConditionOkayIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            bus.DidNotReceive()
               .PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SendMessage_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            linesSourceManager.Lines.Returns(CreateLines());
            linesSourceManager.CostPerLine.Returns(CreateCostPerLine());

            // Act
            sut.Calculate();

            // Assert
            bus.Received()
               .PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsLines_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            IEnumerable <ILine> expected = CreateLines();
            linesSourceManager.Lines.Returns(expected);
            linesSourceManager.CostPerLine.Returns(CreateCostPerLine());

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected,
                         sut.Lines);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsCostPerLine_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            int[] expected = CreateCostPerLine();

            linesSourceManager.Lines.Returns(CreateLines());
            linesSourceManager.CostPerLine.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected,
                         sut.CostPerLine);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsLineDtos_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ILine[] expected = CreateLines();
            linesSourceManager.Lines.Returns(expected);
            linesSourceManager.CostPerLine.Returns(CreateCostPerLine());

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(expected.Length,
                         sut.LineDtos.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsSettings_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull, Frozen] IRacetrackSettingsSourceManager racetrackSettingsSourceManager,
            [NotNull, Frozen] IRacetrackSettingsSource racetrackSettingsSource,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            linesSourceManager.Lines.Returns(CreateLines());
            linesSourceManager.CostPerLine.Returns(CreateCostPerLine());

            racetrackSettingsSourceManager.Source.Returns(racetrackSettingsSource);

            // Act
            sut.Calculate();

            // Assert
            Assert.Equal(racetrackSettingsSource,
                         sut.Settings);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForLinesIsEmpty(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            var linesInvalid = new ILine[0];
            var costPerLinesValid = new[]
                                    {
                                        1
                                    };

            // Act
            bool actual = sut.IsAllConditionOkay(linesInvalid,
                                                 costPerLinesValid);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForLinesIsEmpty(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            var linesInvalid = new ILine[0];
            var costPerLinesValid = new[]
                                    {
                                        1
                                    };

            // Act
            sut.IsAllConditionOkay(linesInvalid,
                                   costPerLinesValid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForCostPerLinesIsEmpty(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ILine[] linesValid = CreateLines();
            var costPerLinesInvalid = new int[0];

            // Act
            bool actual = sut.IsAllConditionOkay(linesValid,
                                                 costPerLinesInvalid);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForCostPerLinesIsEmpty(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ILine[] linesValid = CreateLines();
            var costPerLinesInvalid = new int[0];

            // Act
            sut.IsAllConditionOkay(linesValid,
                                   costPerLinesInvalid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForLinesDoNotMatchCostPerLines(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ILine[] linesValid = CreateLines();
            var costPerLinesInvalid = new[]
                                      {
                                          1
                                      };

            // Act
            bool actual = sut.IsAllConditionOkay(linesValid,
                                                 costPerLinesInvalid);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForLinesDoNotMatchCostPerLines(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ILine[] linesValid = CreateLines();
            var costPerLinesInvalid = new[]
                                      {
                                          1
                                      };

            // Act
            sut.IsAllConditionOkay(linesValid,
                                   costPerLinesInvalid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        private static bool IsValidCostMatrixCalculateMessage(
            CostMatrixCalculateMessage x,
            LineDto[] lineDtos,
            IRacetrackSettingsSource settings)
        {
            return x.LineDtos == lineDtos &&
                   Math.Abs(x.TurnRadiusForPort - settings.TurnRadiusForPort) < Tolerance &&
                   Math.Abs(x.TurnRadiusForStarboard - settings.TurnRadiusForStarboard) < Tolerance &&
                   x.IsPortTurnAllowed == settings.IsPortTurnAllowed &&
                   x.IsStarboardTurnAllowed == settings.IsStarboardTurnAllowed;
        }

        private IRacetrackSettingsSource CreateSettings()
        {
            return new RacetrackSettingsSource(30.0,
                                               30.0,
                                               true,
                                               true);
        }

        private static int[] CreateCostPerLine()
        {
            return new[]
                   {
                       1,
                       2,
                       3,
                       4
                   };
        }

        private static int[][] CreateIntegerMatrix()
        {
            return new[]
                   {
                       new[]
                       {
                           1,
                           2,
                           3,
                           4
                       },
                       new[]
                       {
                           1,
                           2,
                           3,
                           4
                       },
                       new[]
                       {
                           1,
                           2,
                           3,
                           4
                       },
                       new[]
                       {
                           1,
                           2,
                           3,
                           4
                       }
                   };
        }

        private static double[][] CreateDoubleMatrix()
        {
            return new[]
                   {
                       new[]
                       {
                           1.0,
                           2.0,
                           3.0,
                           4.0
                       },
                       new[]
                       {
                           1.0,
                           2.0,
                           3.0,
                           4.0
                       },
                       new[]
                       {
                           1.0,
                           2.0,
                           3.0,
                           4.0
                       },
                       new[]
                       {
                           1.0,
                           2.0,
                           3.0,
                           4.0
                       }
                   };
        }

        private ILine[] CreateLines()
        {
            return new[]
                   {
                       Substitute.For <ILine>(),
                       Substitute.For <ILine>()
                   };
        }

        private LineDto[] CreateLineDtos()
        {
            return new[]
                   {
                       new LineDto(),
                       new LineDto()
                   };
        }
    }
}