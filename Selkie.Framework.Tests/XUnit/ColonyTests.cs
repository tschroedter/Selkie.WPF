using System;
using System.Threading.Tasks;
using EasyNetQ;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Services.Aco.Common.Messages;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class ColonyTests
    {
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
        public void ColonyStartRequestHandler_CallsCreateColony_WhenCalled([NotNull, Frozen] IServiceProxy proxy,
                                                                           [NotNull] Colony sut,
                                                                           [NotNull] ColonyStartRequestMessage message)
        {
            // Arrange
            proxy.IsColonyCreated.Returns(true);

            // Act
            sut.ColonyStartRequestHandler(message);

            // Assert
            proxy.Received().CreateColony();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyStartRequestHandler_CallsStart_WhenCalled([NotNull, Frozen] IServiceProxy proxy,
                                                                    [NotNull] Colony sut,
                                                                    [NotNull] ColonyStartRequestMessage message)
        {
            // Arrange
            proxy.IsColonyCreated.Returns(true);

            // Act
            sut.ColonyStartRequestHandler(message);

            // Assert
            proxy.Received().Start();
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
        public void ColonyPheromonesRequestHandler_SendsMessage_WhenCalled([NotNull, Frozen] IBus bus,
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
        public void Constructor_SubscribeToColonyStartRequestMessage_WhenCreated([NotNull, Frozen] IBus bus,
                                                                                 [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Func <ColonyStartRequestMessage, Task>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToColonyStopRequestMessage_WhenCreated([NotNull, Frozen] IBus bus,
                                                                                [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Func <ColonyStopRequestMessage, Task>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribeToColonyPheromonesRequestMessage_WhenCreated([NotNull, Frozen] IBus bus,
                                                                                      [NotNull] Colony sut)
        {
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Func <ColonyPheromonesRequestMessage, Task>>());
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

        [Theory]
        [AutoNSubstituteData]
        public void SleepTime_Roundtrip([NotNull] Colony sut)
        {
            // Arrange
            Assert.Equal(Colony.SleepTimeOneSecond,
                         sut.SleepTimeInMs);

            // Act
            sut.SetSleepTimeInMs(2000);

            // Assert
            Assert.Equal(2000,
                         sut.SleepTimeInMs);
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