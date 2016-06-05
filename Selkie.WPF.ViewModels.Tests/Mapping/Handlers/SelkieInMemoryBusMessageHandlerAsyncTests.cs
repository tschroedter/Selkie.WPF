using System;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels.Tests.Mapping.NUnit.Handlers
{
    [TestFixture]
    internal sealed class SelkieInMemoryBusMessageHandlerAsyncTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();

            m_Sut = new TestSelkieInMemoryBusMessageHandlerAsync(m_Logger,
                                                                 m_Bus);
        }

        private ISelkieLogger m_Logger;
        private ISelkieInMemoryBus m_Bus;
        private TestSelkieInMemoryBusMessageHandlerAsync m_Sut;

        private class TestSelkieInMemoryBusMessageHandlerAsync
            : SelkieInMemoryBusMessageHandlerAsync <TestMessage>
        {
            public TestSelkieInMemoryBusMessageHandlerAsync([NotNull] ISelkieLogger logger,
                                                            [NotNull] ISelkieInMemoryBus bus)
                : base(logger,
                       bus)
            {
            }

            public bool WasCalledHandle { get; private set; }

            public override void Handle(TestMessage message)
            {
                WasCalledHandle = true;
            }
        }

        private class TestMessage
        {
        }

        [Test]
        public void Constructor_SubscribesToMessage_WhenCalled()
        {
            // Arrange
            string subscriptionId = m_Sut.GetType().FullName;

            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(subscriptionId,
                                            Arg.Any <Action <TestMessage>>());
        }

        [Test]
        public void Handle_IsCalled_ForMessage()
        {
            // Arrange
            var message = new TestMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            Assert.True(m_Sut.WasCalledHandle);
        }
    }
}