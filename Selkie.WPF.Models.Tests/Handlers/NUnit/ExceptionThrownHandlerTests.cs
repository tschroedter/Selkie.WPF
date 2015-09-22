using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Messages;
using Selkie.Windsor;
using Selkie.WPF.Models.Handlers;

namespace Selkie.WPF.Models.Tests.Handlers.NUnit
{
    //ncrunch: no coverage start 
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionThrownHandlerTests
    {
        private ExceptionThrownMessage CreateMessage()
        {
            return new ExceptionThrownMessage
                   {
                       Invocation = "Invocation",
                       Message = "Message",
                       StackTrace = "StackTrace"
                   };
        }

        [Test]
        public void FunctionUnderTest_ExpectedResult_UnderCondition()
        {
            // Arrange
            ExceptionThrownMessage message = CreateMessage();
            var logger = Substitute.For <ISelkieLogger>();
            var sut = new ExceptionThrownHandler(logger);

            // Act
            sut.Handle(message);

            // Assert
            logger.Received().Error(Arg.Is <string>(x => x.Contains("Invocation")));
        }
    }
}