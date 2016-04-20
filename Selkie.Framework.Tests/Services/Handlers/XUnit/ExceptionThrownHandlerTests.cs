using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Services.Handlers;
using Selkie.XUnit.Extensions;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.Services.Handlers.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class ExceptionThrownHandlerTests
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