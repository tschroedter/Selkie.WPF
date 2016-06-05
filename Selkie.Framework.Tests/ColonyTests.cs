using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Framework.Messages;
using Selkie.NUnit.Extensions;
using Selkie.Services.Aco.Common.Messages;

namespace Selkie.Framework.Tests.XUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ColonyTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void ColonyPheromonesRequestHandler_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieBus bus,
                                                                           [NotNull] Colony sut,
                                                                           [NotNull] ColonyPheromonesRequestMessage
                                                                               message)
        {
            // Arrange
            // Act
            sut.ColonyPheromonesRequestHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <PheromonesRequestMessage>());
        }


        [Theory]
        [AutoNSubstituteData]
        public void ColonyStartRequestHandler_CallsManager_WhenCalled(
            [NotNull, Frozen] ICostMatrixCalculationManager manager,
            [NotNull] Colony sut,
            [NotNull] ColonyStartRequestMessage message)
        {
            // Arrange
            // Act
            sut.ColonyStartRequestHandler(message);

            // Assert
            manager.Received().Calculate();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyStopRequestHandler_CallsStop_WhenCalled([NotNull, Frozen] IServiceProxy proxy,
                                                                  [NotNull] Colony sut,
                                                                  [NotNull] ColonyStopRequestMessage message)
        {
            // Arrange
            // Act
            sut.ColonyStopRequestHandler(message);

            // Assert
            proxy.Received().Stop();
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToColonyPheromonesRequestMessage_WhenCreated(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyPheromonesRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToColonyStartRequestMessage_WhenCreated(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyStartRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToColonyStopRequestMessage_WhenCreated(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyStopRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixCalculatedMessageHandler_CallsCreateColony_WhenCalled(
            [NotNull, Frozen] IColonyParameters colonyParameters,
            [NotNull, Frozen] IServiceProxy proxy,
            [NotNull] Colony sut,
            [NotNull] CostMatrixCalculatedMessage message)
        {
            // Arrange
            proxy.IsColonyCreated.Returns(true);

            // Act
            sut.CostMatrixCalculatedMessageHandler(message);

            // Assert
            proxy.Received().CreateColony(colonyParameters);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixCalculatedMessageHandler_CallsFactory_WhenCalled(
            [NotNull, Frozen] IColonyParametersFactory factory,
            [NotNull, Frozen] IAntSettingsSource source,
            [NotNull] Colony sut,
            [NotNull] CostMatrixCalculatedMessage message)
        {
            // Arrange
            // Act
            sut.CostMatrixCalculatedMessageHandler(message);

            // Assert
            factory.Received().Create(message.Matrix,
                                      message.CostPerFeature,
                                      source.IsFixedStartNode,
                                      source.FixedStartNode);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostMatrixCalculatedMessageHandler_CallsStart_WhenCalled([NotNull, Frozen] IServiceProxy proxy,
                                                                             [NotNull] Colony sut,
                                                                             [NotNull] CostMatrixCalculatedMessage
                                                                                 message)
        {
            // Arrange
            proxy.IsColonyCreated.Returns(true);

            // Act
            sut.CostMatrixCalculatedMessageHandler(message);

            // Assert
            proxy.Received().Start();
        }

        [Theory]
        [AutoNSubstituteData]
        public void IsRunning_ReturnsValueFromProxy_WhenCalled([NotNull, Frozen] IServiceProxy proxy,
                                                               [NotNull] Colony sut)
        {
            // Arrange
            proxy.IsRunning.Returns(true);

            // Act
            // Assert
            Assert.True(sut.IsRunning);
        }

        [Theory]
        [AutoNSubstituteData]
        public void SleepTime_Roundtrip([NotNull] Colony sut)
        {
            // Arrange
            Assert.AreEqual(Colony.SleepTimeOneSecond,
                            sut.SleepTimeInMs);

            // Act
            sut.SetSleepTimeInMs(2000);

            // Assert
            Assert.AreEqual(2000,
                            sut.SleepTimeInMs);
        }

        [Theory]
        [AutoNSubstituteData]
        public void SleepWaitAndDo_CallsDoSomething_WhenCalledBreakIfTrueIsAlwaysFalse([NotNull] Colony sut)
        {
            // Arrange
            var test = new TestDoSomething();
            sut.SetSleepTimeInMs(1);

            // Act
            sut.SleepWaitAndDo(test.IsAlwaysFalse(),
                               test.DoSomething());

            // Assert
            Assert.True(test.WasCalledDoSomething);
        }

        [Theory]
        [AutoNSubstituteData]
        public void SleepWaitAndDo_DoesNotCallsDoSomething_WhenCalledBreakIfTrueIsAlwaysTrue([NotNull] Colony sut)
        {
            // Arrange
            var test = new TestDoSomething();
            sut.SetSleepTimeInMs(1);

            // Act
            sut.SleepWaitAndDo(test.IsAlwaysTrue(),
                               test.DoSomething());

            // Assert
            Assert.False(test.WasCalledDoSomething);
        }

        private class TestDoSomething
        {
            public bool WasCalledDoSomething;

            public Action DoSomething()
            {
                return () => WasCalledDoSomething = true;
            }

            public Func <bool> IsAlwaysFalse()
            {
                return () => false;
            }

            public Func <bool> IsAlwaysTrue()
            {
                return () => true;
            }
        }
    }
}