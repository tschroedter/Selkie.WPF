using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Aco;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Services.Aco.Common.Messages;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.Aco.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class ServiceProxyTests
    {
        private const double Tolerance = 0.01;

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToCreatedColonyMessage_WhenCreated([NotNull, Frozen] ISelkieBus bus,
                                                                            [NotNull] ServiceProxy sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <CreatedColonyMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToStartedMessage_WhenCreated([NotNull, Frozen] ISelkieBus bus,
                                                                      [NotNull] ServiceProxy sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <StartedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToStoppedMessage_WhenCreated([NotNull, Frozen] ISelkieBus bus,
                                                                      [NotNull] ServiceProxy sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <StoppedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToFinishedMessage_WhenCreated([NotNull, Frozen] ISelkieBus bus,
                                                                       [NotNull] ServiceProxy sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <FinishedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Stop_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                 [NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.Stop();

            // Assert
            bus.Received().PublishAsync(Arg.Any <StopRequestMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreatedColonyHandler_SetsIsColonyCreatedToTrue_WhenCalled([NotNull] ServiceProxy sut,
                                                                              [NotNull] CreatedColonyMessage message)
        {
            // Arrange
            // Act
            sut.CreatedColonyHandler(message);

            // Assert
            Assert.True(sut.IsColonyCreated);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreatedColonyHandler_SetsIsFinishedToFalse_WhenCalled([NotNull] ServiceProxy sut,
                                                                          [NotNull] CreatedColonyMessage message)
        {
            // Arrange
            // Act
            sut.CreatedColonyHandler(message);

            // Assert
            Assert.False(sut.IsFinished);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StartedHandler_SetsIsRunningToTrue_WhenCalled([NotNull] ServiceProxy sut,
                                                                  [NotNull] StartedMessage message)
        {
            // Arrange
            // Act
            sut.StartedHandler(message);

            // Assert
            Assert.True(sut.IsRunning);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StartedHandler_SetsIsFinishedToFalse_WhenCalled([NotNull] ServiceProxy sut,
                                                                    [NotNull] StartedMessage message)
        {
            // Arrange
            // Act
            sut.StartedHandler(message);

            // Assert
            Assert.False(sut.IsFinished);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StartedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                           [NotNull] ServiceProxy sut,
                                                           [NotNull] StartedMessage message)
        {
            // Arrange
            // Act
            sut.StartedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyStartedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void StoppedHandler_SetsIsRunningToFalse_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                   [NotNull] ServiceProxy sut,
                                                                   [NotNull] StoppedMessage message)
        {
            // Arrange
            // Act
            sut.StoppedHandler(message);

            // Assert
            Assert.False(sut.IsRunning);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StoppedHandler_SetsIsColonyCreatedToFalse_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                         [NotNull] ServiceProxy sut,
                                                                         [NotNull] StoppedMessage message)
        {
            // Arrange
            // Act
            sut.StoppedHandler(message);

            // Assert
            Assert.False(sut.IsColonyCreated);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StoppedHandler_SetsIsFinishedToFalse_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                    [NotNull] ServiceProxy sut,
                                                                    [NotNull] StoppedMessage message)
        {
            // Arrange
            // Act
            sut.StoppedHandler(message);

            // Assert
            Assert.False(sut.IsFinished);
        }

        [Theory]
        [AutoNSubstituteData]
        public void StoppedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                           [NotNull] ServiceProxy sut,
                                                           [NotNull] StoppedMessage message)
        {
            // Arrange
            // Act
            sut.StoppedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyStoppedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void FinishedHandler_SetsIsRunningToFalse_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                    [NotNull] ServiceProxy sut,
                                                                    [NotNull] FinishedMessage message)
        {
            // Arrange
            // Act
            sut.FinishedHandler(message);

            // Assert
            Assert.False(sut.IsRunning);
        }

        [Theory]
        [AutoNSubstituteData]
        public void FinishedHandler_SetsIsFinishedToTrue_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                    [NotNull] ServiceProxy sut,
                                                                    [NotNull] FinishedMessage message)
        {
            // Arrange
            // Act
            sut.FinishedHandler(message);

            // Assert
            Assert.True(sut.IsFinished);
        }

        [Theory]
        [AutoNSubstituteData]
        public void FinishedHandler_SetsIsColonyCreatedToFalse_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                          [NotNull] ServiceProxy sut,
                                                                          [NotNull] FinishedMessage message)
        {
            // Arrange
            // Act
            sut.FinishedHandler(message);

            // Assert
            Assert.False(sut.IsColonyCreated);
        }

        [Theory]
        [AutoNSubstituteData]
        public void FinishedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                            [NotNull] ServiceProxy sut,
                                                            [NotNull] FinishedMessage message)
        {
            // Arrange
            // Act
            sut.FinishedHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyFinishedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Start_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                  [NotNull] ServiceProxy sut)
        {
            // Arrange
            SetIsRunningToFalse(sut);
            SetIsColonyCreatedToTrue(sut);

            // Act
            sut.Start();

            // Assert
            bus.Received().PublishAsync(Arg.Is <StartMessage>(x => x.Times == 2000));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Start_DoesNotSendsMessage_WhenIsColonyCreatedIsFalse([NotNull, Frozen] ISelkieBus bus,
                                                                         [NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.Start();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Is <StartMessage>(x => x.Times == 1000));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Start_DoesNotSendsMessage_WhenIsRunningIsTrue([NotNull, Frozen] ISelkieBus bus,
                                                                  [NotNull] ServiceProxy sut)
        {
            // Arrange
            SetIsColonyCreatedToTrue(sut);
            SetIsRunningToFalse(sut);

            // Act
            sut.Start();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Is <StartMessage>(x => x.Times == 1000));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ICostMatrixSourceManager costMatrixSourceManager,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.CreateColony();

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <CreateColonyMessage>(x => x.CostMatrix == costMatrixSourceManager.Matrix &&
                                                               x.CostPerLine.SequenceEqual(
                                                                                           linesSourceManager
                                                                                               .CostPerLine)));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_DoesNotSendsMessage_WhenMatrixIsEmpty(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] ICostMatrixSourceManager costMatrixSourceManager,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            costMatrixSourceManager.Matrix.Returns(new int[0][]);
            // Act
            sut.CreateColony();

            // Assert
            bus.DidNotReceive().PublishAsync(Arg.Any <CreateColonyMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SetsIsColonyCreatedToFalse_WhenCalled([NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.CreateColony();

            // Assert
            Assert.False(sut.IsColonyCreated);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SetsIsRunningToFalse_WhenCalled([NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.CreateColony();

            // Assert
            Assert.False(sut.IsRunning);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_CallsLogCostMatrix_WhenCalled(
            [NotNull, Frozen] IAcoProxyLogger logger,
            [NotNull, Frozen] ICostMatrixSourceManager costMatrixSourceManager,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.CreateColony();

            // Assert
            logger.Received().LogCostMatrix(Arg.Any <int[][]>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_CallsLogCostPerLine_WhenCalled(
            [NotNull, Frozen] IAcoProxyLogger logger,
            [NotNull, Frozen] ILinesSourceManager linesSourceManager,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            // Act
            sut.CreateColony();

            // Assert
            logger.Received().LogCostPerLine(Arg.Any <int[]>());
        }

        private void SetIsRunningToFalse(ServiceProxy sut)
        {
            var message = new FinishedMessage();

            sut.FinishedHandler(message);
        }

        private void SetIsColonyCreatedToTrue(ServiceProxy sut)
        {
            var message = new CreatedColonyMessage();

            sut.CreatedColonyHandler(message);
        }
    }
}