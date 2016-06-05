using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Status;

namespace Selkie.WPF.Models.Tests.Status
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class ExceptionThrownModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();

            m_Sut = new ExceptionThrownModel(m_Bus);
        }

        private ISelkieInMemoryBus m_Bus;
        private ExceptionThrownModel m_Sut;

        [Test]
        public void ClearError_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyExceptionThrownMessage
                          {
                              Text = "Text"
                          };

            m_Sut.ColonyExceptionThrownHandler(message);

            m_Bus.ClearReceivedCalls();

            // Act
            m_Sut.ClearExceptionThrownHandler(new ExceptionThrownClearErrorMessage());


            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ExceptionThrownChangedMessage>(x => x.Text == string.Empty));
        }

        [Test]
        public void ClearExceptionThrownHandler_SetsLastErrorToEmpty_WhenCalled()
        {
            // Arrange
            var message = new ColonyExceptionThrownMessage
                          {
                              Text = "Text"
                          };

            m_Sut.ColonyExceptionThrownHandler(message);

            // Act
            m_Sut.ClearExceptionThrownHandler(new ExceptionThrownClearErrorMessage());

            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.LastError);
        }

        [Test]
        public void ColonyExceptionThrownHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            const string expected = "Text";
            var message = new ColonyExceptionThrownMessage
                          {
                              Text = expected
                          };

            // Act
            m_Sut.ColonyExceptionThrownHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ExceptionThrownChangedMessage>(x => x.Text == expected));
        }

        [Test]
        public void ColonyExceptionThrownHandler_SetsLastError_WhenCalled()
        {
            // Arrange
            const string expected = "Text";
            var message = new ColonyExceptionThrownMessage
                          {
                              Text = expected
                          };

            // Act
            m_Sut.ColonyExceptionThrownHandler(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.LastError);
        }

        [Test]
        public void Constructor_SubscribesToClearExceptionThrownMessage_WhenCalled()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Action <ExceptionThrownClearErrorMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyStatusMessage_WhenCalled()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Action <ColonyExceptionThrownMessage>>());
        }

        [Test]
        public void LastError_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.LastError);
        }
    }
}