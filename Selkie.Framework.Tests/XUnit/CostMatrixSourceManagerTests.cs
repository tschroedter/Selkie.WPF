using System;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class CostMatrixSourceManagerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToCostMatrixChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixSourceManager sut)
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
            [NotNull] CostMatrixSourceManager sut)
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
            [NotNull] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyRacetrackSettingsChangedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_CallsCalculate_WhenCalled(
            [NotNull, Frozen] ICalculateCostMatrixManager manager,
            [NotNull] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            manager.Received().Calculate();
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SetsMatrix_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Matrix);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesChangedHandler_CallsManagersCalculateMetod_WhenCalled(
            [NotNull, Frozen] ICalculateCostMatrixManager manager,
            [NotNull] CostMatrixSourceManager sut,
            [NotNull] ColonyLinesChangedMessage message)
        {
            // Arrange
            // Act
            sut.ColonyLinesChangedHandler(message);

            // Assert
            manager.Received().Calculate();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyRacetrackSettingsChangedHandler_CallsManagersCalculateMethod_WhenCalled(
            [NotNull, Frozen] ICalculateCostMatrixManager manager,
            [NotNull] CostMatrixSourceManager sut,
            [NotNull] ColonyRacetrackSettingsChangedMessage message)
        {
            // Arrange
            // Act
            sut.ColonyRacetrackSettingsChangedHandler(message);

            // Assert
            manager.Received().Calculate();
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendsColonyCostMatrixChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CostMatrixSourceManager sut,
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
            [NotNull] CostMatrixSourceManager sut,
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
            [NotNull] CostMatrixSourceManager sut,
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