using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class CostMatrixSourceManagerTests
    {
        private const double Tolerance = 0.1;

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToCostMatrixChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <CostMatrixChangedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyLinesChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyLinesChangedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyRacetrackSettingsChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyRacetrackSettingsChangedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToCostMatrixGetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().PublishAsync(Arg.Any <CostMatrixGetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SetsMatrix_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Matrix);
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendRacetrackSettingsSetMessage_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] IRacetrackSettingsSourceManager manager,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.SendRacetrackSettingsSetMessage();

            // Assert
            bus.Received()
               .PublishAsync(
                             Arg.Is <RacetrackSettingsSetMessage>(
                                                                  x =>
                                                                  Math.Abs(x.TurnRadiusInMetres -
                                                                           manager.Source.TurnRadius) < 0.01 &&
                                                                  x.IsPortTurnAllowed ==
                                                                  manager.Source.IsPortTurnAllowed &&
                                                                  x.IsStarboardTurnAllowed ==
                                                                  manager.Source.IsStarboardTurnAllowed));
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendLinesSetMessage_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.SendLinesSetMessage();

            // Assert
            bus.Received().PublishAsync(Arg.Is <LinesSetMessage>(x => x.LineDtos.Any()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDtos_ForLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
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
            [NotNull, Frozen] CostMatrixSourceManager sut)
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
        public void SendCostMatrixCalculateMessage_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.SendCostMatrixCalculateMessage();

            // Assert
            bus.Received().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesChangedHandler_CallsRecalculateCostMatrix_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut,
            [NotNull] ColonyLinesChangedMessage message)
        {
            // Arrange
            // Act
            sut.ColonyLinesChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyRacetrackSettingsChangedHandler_CallsRecalculateCostMatrix_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut,
            [NotNull] ColonyRacetrackSettingsChangedMessage message)
        {
            // Arrange
            // Act
            sut.ColonyRacetrackSettingsChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecalculateCostMatrix_SendsRacetrackSettingsSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.RecalculateCostMatrix();

            // Assert
            bus.Received().PublishAsync(Arg.Any <RacetrackSettingsSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecalculateCostMatrix_SendsLinesSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.RecalculateCostMatrix();

            // Assert
            bus.Received().PublishAsync(Arg.Any <LinesSetMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void RecalculateCostMatrix_SendsCostMatrixCalculateMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            sut.RecalculateCostMatrix();

            // Assert
            bus.Received().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut,
            [NotNull] CostMatrixChangedMessage message)
        {
            // Arrange
            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyCostMatrixChangedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_DoesNotSendsMessage_WhenMatrixIsNull(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CostMatrixSourceManager sut,
            [NotNull] CostMatrixChangedMessage message)
        {
            // Arrange
            message.Matrix = null;

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <ColonyCostMatrixChangedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixChangedHandler_LogsMessage_WhenMatrixIsNull(
            [NotNull, Frozen] ISelkieLogger logger,
            [NotNull, Frozen] CostMatrixSourceManager sut,
            [NotNull] CostMatrixChangedMessage message)
        {
            // Arrange
            message.Matrix = null;

            // Act
            sut.CostMatrixChangedHandler(message);

            // Assert
            logger.Received().Warn(Arg.Any <string>());
        }
    }
}