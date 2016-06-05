using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Services.Handlers;
using Selkie.NUnit.Extensions;
using Selkie.Services.Aco.Common.Messages;

namespace Selkie.Framework.Tests.Services.Handlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class PheromonesHandlerTests
    {
        private const double Tolerance = 0.01;

        [Theory]
        [AutoNSubstituteData]
        public void Handle_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
                                                   [NotNull] PheromonesMessage message,
                                                   [NotNull] PheromonesHandler sut)
        {
            sut.Handle(message);

            bus.Received()
               .PublishAsync(Arg.Is <ColonyPheromonesMessage>(x => x.Values.SequenceEqual(message.Values) &&
                                                                   Math.Abs(x.Minimum - message.Minimum) < Tolerance &&
                                                                   Math.Abs(x.Maximum - message.Maximum) < Tolerance &&
                                                                   Math.Abs(x.Average - message.Average) < Tolerance));
        }
    }
}