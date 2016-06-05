using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.EasyNetQ;
using Selkie.Framework.Aco;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.NUnit.Extensions;
using Selkie.Services.Aco.Common.Messages;

namespace Selkie.Framework.Tests.Aco
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ServiceProxyTests
    {
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
        public void Constructor_SubscribeToFinishedMessage_WhenCreated([NotNull, Frozen] ISelkieBus bus,
                                                                       [NotNull] ServiceProxy sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <FinishedMessage>>());
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
        public void CreateColony_CallsLogCostMatrix_WhenCalled(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull, Frozen] IAcoProxyLogger logger,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = 0;

            // Act
            sut.CreateColony(colonyParameters);

            // Assert
            logger.Received().LogCostMatrix(Arg.Any <int[][]>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_CallsLogCostPerFeature_WhenCalled(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull, Frozen] IAcoProxyLogger logger,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = 0;

            // Act
            sut.CreateColony(colonyParameters);

            // Assert
            logger.Received().LogCostPerFeature(Arg.Any <int[]>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_DoesNotSendsMessage_WhenColonyParametersAreInvalid(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull, Frozen] IColonyParametersValidator validator,
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            validator.When(x => x.Validate(Arg.Any <IColonyParameters>()))
                     .Do(ThrowArgumentException);

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.CreateColony(colonyParameters));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SendsMessage_WhenCalled(
            [NotNull, Frozen] IColonyParameters colonyParameters,
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = 0;

            // Act
            sut.CreateColony(colonyParameters);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <CreateColonyMessage>(x =>
                                                          x.CostMatrix.SequenceEqual(colonyParameters.CostMatrix) &&
                                                          x.CostPerFeature.SequenceEqual(colonyParameters.CostPerFeature) &&
                                                          x.IsFixedStartNode == false &&
                                                          x.FixedStartNode == colonyParameters.FixedStartNode));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SetsIsColonyCreatedToFalse_WhenCalled(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = 0;

            // Act
            sut.CreateColony(colonyParameters);

            // Assert
            Assert.False(sut.IsColonyCreated);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateColony_SetsIsRunningToFalse_WhenCalled(
            [NotNull] IColonyParameters colonyParameters,
            [NotNull] ServiceProxy sut)
        {
            // Arrange
            colonyParameters.CostMatrix = CreateValidMatrix();
            colonyParameters.CostPerFeature = CreateValidCostPerFeature();
            colonyParameters.IsFixedStartNode = false;
            colonyParameters.FixedStartNode = 0;

            // Act
            sut.CreateColony(colonyParameters);


            // Assert
            Assert.False(sut.IsRunning);
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
        public void FinishedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
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
        public void StartedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
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
        public void StoppedHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
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

        private static int[] CreateValidCostPerFeature()
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

        private void SetIsColonyCreatedToTrue(ServiceProxy sut)
        {
            var message = new CreatedColonyMessage();

            sut.CreatedColonyHandler(message);
        }

        private void SetIsRunningToFalse(ServiceProxy sut)
        {
            var message = new FinishedMessage();

            sut.FinishedHandler(message);
        }

        private void ThrowArgumentException(CallInfo callInfo)
        {
            throw new ArgumentException("Test");
        }
    }
}