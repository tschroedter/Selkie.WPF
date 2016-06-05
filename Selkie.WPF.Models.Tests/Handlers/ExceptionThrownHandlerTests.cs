using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Models.Handlers;

namespace Selkie.WPF.Models.Tests.Handlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionThrownHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <IExceptionThrownMessageToStringConverter>();

            m_Sut = new ExceptionThrownHandler(m_Logger,
                                               m_Bus,
                                               m_Converter);
        }

        private ISelkieLogger m_Logger;
        private ISelkieInMemoryBus m_Bus;
        private ExceptionThrownHandler m_Sut;
        private IExceptionThrownMessageToStringConverter m_Converter;

        private ExceptionThrownMessage CreateMessage()
        {
            var information = new ExceptionInformation
                              {
                                  Invocation = "Invocation",
                                  Message = "Message",
                                  StackTrace = "StackTrace"
                              };

            return new ExceptionThrownMessage
                   {
                       Exception = information
                   };
        }

        [Test]
        public void Handle_LogsError_WhenCalled()
        {
            // Arrange
            m_Converter.Convert(Arg.Any <ExceptionThrownMessage>()).Returns("Exception");
            ExceptionThrownMessage message = CreateMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            m_Logger.Received().Error(Arg.Is <string>(x => x == "Exception"));
        }

        [Test]
        public void Handle_SendsMessage_WhenCalled()
        {
            // Arrange
            ExceptionThrownMessage message = CreateMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <StatusMessage>());
        }
    }
}