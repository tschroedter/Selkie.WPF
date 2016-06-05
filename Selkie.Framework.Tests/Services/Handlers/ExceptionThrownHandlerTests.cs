using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Services.Handlers;
using Selkie.NUnit.Extensions;

namespace Selkie.Framework.Tests.Services.Handlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionThrownHandlerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Handle_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
                                                   [NotNull] ExceptionThrownMessage message,
                                                   [NotNull] ExceptionThrownHandler sut)
        {
            sut.Handle(message);

            bus.Received()
               .PublishAsync(Arg.Is <ColonyExceptionThrownMessage>(x => x.Text == message.Exception.Message));
        }
    }
}