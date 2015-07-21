using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Services.Handlers.XUnit

{
    [ExcludeFromCodeCoverage]
    public class BaseHandlerTests
    {
        [Fact]
        public void FunctionUnderTest_ExpectedResult_UnderCondition()
        {
            // Arrange


            // Act


            // Assert
        }

        [Theory]
        [AutoNSubstituteData]
        public void Start_SubscribesToMessage_WhenCalled([NotNull] [Frozen] IBus bus,
                                                         [NotNull] TestSelkieBaseHandler sut)
        {
            string subscriptionId = sut.GetType().ToString();

            sut.Start();

            bus.Received().SubscribeAsync(subscriptionId,
                                          Arg.Any <Func <TestMessage, Task>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Start_LogsMessage_WhenCalled([NotNull] [Frozen] ILogger logger,
                                                 [NotNull] TestSelkieBaseHandler sut)
        {
            sut.Start();

            logger.Received().Info(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Stop_LogsMessage_WhenCalled([NotNull] [Frozen] ILogger logger,
                                                [NotNull] TestSelkieBaseHandler sut)
        {
            sut.Stop();

            logger.Received().Info(Arg.Any <string>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Handle_CallsHandler_WhenCalled([NotNull] TestSelkieBaseHandler sut)
        {
            sut.Handle(new TestMessage());

            Assert.True(sut.HandleWasCalled);
        }

        public class TestSelkieBaseHandler : BaseHandler <TestMessage>
        {
            public bool HandleWasCalled;

            public TestSelkieBaseHandler([NotNull] ILogger logger,
                                         [NotNull] IBus bus)
                : base(logger,
                       bus)
            {
            }

            // todo need a real TestBus to send/receive messages, so we can make this method protected
            internal override void Handle(TestMessage message)
            {
                HandleWasCalled = true;
            }
        }

        public class TestMessage
        {
        }
    }
}