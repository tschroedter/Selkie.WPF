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
    public sealed class StatusHandlerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Handle_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
                                                   [NotNull] StatusMessage message,
                                                   [NotNull] StatusHandler sut)
        {
            sut.Handle(message);

            bus.Received()
               .PublishAsync(Arg.Is <ColonyStatusMessage>(x => x.Text == message.Text));
        }
    }
}