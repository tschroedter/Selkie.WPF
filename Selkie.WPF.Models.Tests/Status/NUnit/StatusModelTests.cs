using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Status;

namespace Selkie.WPF.Models.Tests.Status.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class StatusModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();

            m_Sut = new StatusModel(m_Bus);
        }

        private ISelkieInMemoryBus m_Bus;
        private StatusModel m_Sut;

        [Test]
        public void ColonyStatusMessageHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            const string expected = "Text";
            var message = new ColonyStatusMessage
                          {
                              Text = expected
                          };

            // Act
            m_Sut.StatusMessageHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <StatusChangedMessage>(x => x.Text == expected));
        }

        [Test]
        public void SubscribeToColonyStatusMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Action <ColonyStatusMessage>>());
        }
    }
}