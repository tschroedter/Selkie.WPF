using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Aop.Messages;
using Selkie.WPF.Models.Handlers;

namespace Selkie.WPF.Models.Tests.Handlers.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionThrownMessageToStringConverterTests
    {
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

        private ExceptionThrownMessage CreateMessageWithInnerExceptions()
        {
            var information = new ExceptionInformation
                              {
                                  Invocation = "Invocation",
                                  Message = "Message",
                                  StackTrace = "StackTrace"
                              };

            var innerInformation = new ExceptionInformation
                                   {
                                       Invocation = "Inner Invocation",
                                       Message = "Inner Message",
                                       StackTrace = "Inner StackTrace"
                                   };

            return new ExceptionThrownMessage
                   {
                       Exception = information,
                       InnerExceptions = new[]
                                         {
                                             innerInformation
                                         }
                   };
        }

        [Test]
        public void Convert_ReturnsString_ForException()
        {
            // Arrange
            var sut = new ExceptionThrownMessageToStringConverter();

            // Act
            string actual = sut.Convert(CreateMessage());

            // Assert
            Assert.AreEqual("Invocation: Invocation\r\nMessage: Message\r\nStackTrace: StackTrace\r\n",
                            actual);
        }

        [Test]
        public void Convert_ReturnsString_ForExceptionWithInnerExceptions()
        {
            // Arrange
            var sut = new ExceptionThrownMessageToStringConverter();

            // Act
            string actual = sut.Convert(CreateMessageWithInnerExceptions());

            // Assert
            Assert.AreEqual("Invocation: Invocation\r\nMessage: Message\r\nStackTrace: StackTrace\r\n" +
                            "Inner Exception:\r\nInvocation: Inner Invocation\r\nMessage: Inner Message\r\nStackTrace: Inner StackTrace\r\n",
                            actual);
        }
    }
}