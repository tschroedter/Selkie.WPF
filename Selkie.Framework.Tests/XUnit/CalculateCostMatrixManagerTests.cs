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
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class CalculateCostMatrixManagerTests // todo rename???
    {
        private const double Tolerance = 0.1;

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SendsRacetrackSettingsSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] IRacetrackSettingsSourceManager manager,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

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
        public void Calculate_SendsLinesSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            bus.Received().PublishAsync(Arg.Is <LinesSetMessage>(x => x.LineDtos.Any()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedRacetrackSettingsChangedMessage_ReturnsFalse_ByDefault(
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsWaitingForChangedMessages_ReturnsFalse_WhenCalculateIsCalled(
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            SetIsReceivedRacetrackSettingsChangedMessageToTrue(sut);

            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedLinesChangedMessage_ReturnsFalse_ByDefault(
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsReceivedLinesChangedMessage_ReturnsFalse_WhenCalculateIsCalled(
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            SetIsReceivedLinesChangedMessageToTrue(sut);

            // Act
            sut.Calculate();

            // Assert
            Assert.False(sut.IsReceivedLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesChangedHandler_SetsIsReceivedLinesChangedMessageToTrue_WhenCalled(
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.ColonyLinesChangedHandler(new ColonyLinesChangedMessage());

            // Assert
            Assert.True(sut.IsReceivedLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyRacetrackSettingsChangedHandler_SetsIsReceivedLinesChangedMessageToTrue_WhenCalled(
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.ColonyRacetrackSettingsChangedHandler(new ColonyRacetrackSettingsChangedMessage());

            // Assert
            Assert.True(sut.IsReceivedRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CheckIfWeCanCalculateRacetrack_SendsCostMatrixCalculateMessage_WhenAllChangedMessageHaveBeenReceived
            (
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            sut.Calculate();
            SetIsReceivedLinesChangedMessageToTrue(sut);
            SetIsReceivedRacetrackSettingsChangedMessageToTrue(sut);

            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            bus.Received().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void
            CheckIfWeCanCalculateRacetrack_SetsIsReceivedRacetrackSettingsChangedMessageToFalse_WhenAllConditionsAreFine
            (
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            SetIsReceivedLinesChangedMessageToTrue(sut);
            SetIsReceivedRacetrackSettingsChangedMessageToTrue(sut);

            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            Assert.False(sut.IsReceivedRacetrackSettingsChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void
            CheckIfWeCanCalculateRacetrack_SetsIsReceivedLinesChangedMessageToFalse_WhenAllConditionsAreFine(
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            SetIsReceivedLinesChangedMessageToTrue(sut);
            SetIsReceivedRacetrackSettingsChangedMessageToTrue(sut);

            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            Assert.False(sut.IsReceivedLinesChangedMessage);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CheckIfWeCanCalculateRacetrack_DoesNothing_ForIsWaitingForChangedMessagesIsFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CheckIfWeCanCalculateRacetrack_DoesNothing_ForIsReceivedRacetrackSettingsChangedMessageFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            sut.Calculate();
            SetIsReceivedLinesChangedMessageToTrue(sut);

            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CheckIfWeCanCalculateRacetrack_DoesNothing_ForIsReceivedLinesChangedMessageFalse(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            sut.Calculate();
            SetIsReceivedRacetrackSettingsChangedMessageToTrue(sut);

            // Act
            sut.CheckIfWeCanCalculateRacetrack();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CostMatrixCalculateMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDtos_ForLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CalculateCostMatrixManager sut)
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
            [NotNull, Frozen] CalculateCostMatrixManager sut)
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

        private void SetIsReceivedRacetrackSettingsChangedMessageToTrue(CalculateCostMatrixManager sut)
        {
            sut.ColonyRacetrackSettingsChangedHandler(new ColonyRacetrackSettingsChangedMessage());
        }

        private void SetIsReceivedLinesChangedMessageToTrue(CalculateCostMatrixManager sut)
        {
            sut.ColonyLinesChangedHandler(new ColonyLinesChangedMessage());
        }
    }
}