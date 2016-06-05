using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.EasyNetQ;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Framework.Messages;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;
using Selkie.NUnit.Extensions;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class CostMatrixCalculationManagerTests
    {
        private const double Tolerance = 0.1;

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
            linesSourceManager.SurveyFeatures.Returns(CreateSurveyFeatures());
            linesSourceManager.CostPerFeature.Returns(CreateCostPerFeature());

            // Act
            sut.Calculate();

            // Assert
            bus.Received()
               .PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsCostPerFeature_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            int[] expected = CreateCostPerFeature();

            linesSourceManager.SurveyFeatures.Returns(CreateSurveyFeatures());
            linesSourceManager.CostPerFeature.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.AreEqual(expected,
                            sut.CostPerFeature);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsCostPerFeature_WhenCalled(
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            int[] expected = CreateCostPerFeature();

            manager.SurveyFeatures.Returns(CreateSurveyFeatures());
            manager.CostPerFeature.Returns(expected);

            // Act
            sut.Calculate();

            // Assert
            Assert.AreEqual(expected,
                            sut.CostPerFeature);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsLines_WhenCalled(
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            ISurveyFeature[] expected = CreateSurveyFeatures().ToArray();

            manager.SurveyFeatures.Returns(expected);
            manager.CostPerFeature.Returns(CreateCostPerFeature());

            // Act
            sut.Calculate();

            // Assert
            Assert.AreEqual(expected,
                            sut.SurveyFeature);
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
            linesSourceManager.SurveyFeatures.Returns(CreateSurveyFeatures());
            linesSourceManager.CostPerFeature.Returns(CreateCostPerFeature());

            racetrackSettingsSourceManager.Source.Returns(racetrackSettingsSource);

            // Act
            sut.Calculate();

            // Assert
            Assert.AreEqual(racetrackSettingsSource,
                            sut.Settings);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SetsSurveyFeatures_ForIsAllConditionOkayIsTrue(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            IEnumerable <ISurveyFeature> expected = CreateSurveyFeatures();
            linesSourceManager.SurveyFeatures.Returns(expected);
            linesSourceManager.CostPerFeature.Returns(CreateCostPerFeature());

            // Act
            sut.Calculate();

            // Assert
            Assert.AreEqual(expected,
                            sut.SurveyFeature);
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
            manager.SurveyFeatures.Returns(CreateSurveyFeatures());
            manager.CostPerFeature.Returns(CreateCostPerFeature());
            message.Matrix = CreateDoubleMatrix();

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
        public void CostMatrixResponseHandler_LogsMessage_WhenMatrixDoesNotMatchLines(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull, Frozen] ILinesSourceManager manager,
            [NotNull] CostMatrixResponseMessage message,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            manager.SurveyFeatures.Returns(CreateSurveyFeatures());
            manager.CostPerFeature.Returns(CreateCostPerFeature());

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
            manager.SurveyFeatures.Returns(CreateSurveyFeatures());
            manager.CostPerFeature.Returns(CreateCostPerFeature());
            message.Matrix = CreateDoubleMatrix();

            sut.Calculate();

            // Act
            sut.CostMatrixResponseHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <CostMatrixCalculatedMessage>(x => x.Matrix == converter.IntegerMatrix &&
                                                                                  x.CostPerFeature.Length ==
                                                                                  manager.CostPerFeature.Count()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateDtos_CallsConvert_ForLines(
            [NotNull] ISurveyFeature[] polylines,
            [NotNull, Frozen] ISurveyFeatureToSurveyFeatureDtoConverter converter,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.CreateDtos(polylines);

            // Assert
            Assert.AreEqual(polylines.Length,
                            converter.Features.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateDtos_ReturnsDtos_ForLines(
            [NotNull] ISurveyFeature[] polylines,
            [NotNull, Frozen] ISurveyFeatureToSurveyFeatureDtoConverter converter,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            IEnumerable <SurveyFeatureDto> actual = sut.CreateDtos(polylines);

            // Assert
            Assert.AreEqual(converter.Dtos,
                            actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateDtos_SetsFeatures_ForLines(
            [NotNull] ISurveyFeature[] polylines,
            [NotNull, Frozen] ISurveyFeatureToSurveyFeatureDtoConverter converter,
            [NotNull, Frozen] CostMatrixCalculationManager sut)
        {
            // Arrange
            // Act
            sut.CreateDtos(polylines);

            // Assert
            Assert.AreEqual(polylines.Length,
                            converter.Features.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForCostPerFeatureIsEmpty(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ISurveyFeature[] polylinesValid = CreateSurveyFeatures();
            var costPerLinesInvalid = new int[0];

            // Act
            sut.IsAllConditionOkay(polylinesValid,
                                   costPerLinesInvalid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForLinesDoNotMatchCostPerFeature(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ISurveyFeature[] polylinesValid = CreateSurveyFeatures();
            var costPerLinesInvalid = new[]
                                      {
                                          1
                                      };

            // Act
            sut.IsAllConditionOkay(polylinesValid,
                                   costPerLinesInvalid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_CallsLogger_ForLinesIsEmpty(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            var polylinesInvalid = new ISurveyFeature[0];
            var costPerLinesValid = new[]
                                    {
                                        1
                                    };

            // Act
            sut.IsAllConditionOkay(polylinesInvalid,
                                   costPerLinesValid);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForCostPerFeatureIsEmpty(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ISurveyFeature[] polylinesValid = CreateSurveyFeatures();
            var costPerLinesInvalid = new int[0];

            // Act
            bool actual = sut.IsAllConditionOkay(polylinesValid,
                                                 costPerLinesInvalid);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForLinesDoNotMatchCostPerFeature(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            ISurveyFeature[] polylinesValid = CreateSurveyFeatures();
            var costPerLinesInvalid = new[]
                                      {
                                          1
                                      };

            // Act
            bool actual = sut.IsAllConditionOkay(polylinesValid,
                                                 costPerLinesInvalid);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsAllConditionOkay_ReturnsFalse_ForLinesIsEmpty(
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            var polylinesInvalid = new ISurveyFeature[0];
            var costPerLinesValid = new[]
                                    {
                                        1
                                    };

            // Act
            bool actual = sut.IsAllConditionOkay(polylinesInvalid,
                                                 costPerLinesValid);

            // Assert
            Assert.False(actual);
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
            Assert.AreEqual(expected,
                            actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendCostMatrixCalculateMessage_SendsMessage_WhenMatrixDoesNotMatchLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixCalculationManager sut)
        {
            // Arrange
            SurveyFeatureDto[] dtos = CreateDtos();
            IRacetrackSettingsSource settings = CreateSettings();


            // Act
            sut.SendCostMatrixCalculateMessage(dtos,
                                               settings);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <CostMatrixCalculateMessage>(x =>
                                                                 IsValidCostMatrixCalculateMessage(x,
                                                                                                   dtos,
                                                                                                   settings)));
        }

        private static int[] CreateCostPerFeature()
        {
            return new[]
                   {
                       1,
                       2,
                       3,
                       4
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

        private static SurveyFeatureDto[] CreateDtos()
        {
            return new[]
                   {
                       new SurveyFeatureDto(),
                       new SurveyFeatureDto()
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

        private static ISurveyFeature[] CreateSurveyFeatures()
        {
            return new[]
                   {
                       Substitute.For <ISurveyFeature>(),
                       Substitute.For <ISurveyFeature>()
                   };
        }

        private static bool IsValidCostMatrixCalculateMessage(
            CostMatrixCalculateMessage x,
            SurveyFeatureDto[] dtos,
            IRacetrackSettingsSource settings)
        {
            return x.SurveyFeatureDtos == dtos &&
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
    }
}