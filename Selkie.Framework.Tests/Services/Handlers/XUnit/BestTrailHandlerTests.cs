using System;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Services.Handlers;
using Selkie.Services.Aco.Common.Messages;
using Selkie.XUnit.Extensions;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.Services.Handlers.XUnit
{
    public sealed class BestTrailHandlerTests
    {
        private const double Tolerance = 0.01;

        [Theory]
        [AutoNSubstituteData]
        public void Handle_SendsMessage_WhenCalled([NotNull, Frozen] ISelkieInMemoryBus bus,
                                                   [NotNull] BestTrailMessage message,
                                                   [NotNull] BestTrailHandler sut)
        {
            sut.Handle(message);

            bus.Received()
               .PublishAsync(Arg.Is <ColonyBestTrailMessage>(x => Math.Abs(x.Alpha - message.Alpha) < Tolerance &&
                                                                  Math.Abs(x.Alpha - message.Alpha) < Tolerance &&
                                                                  Math.Abs(x.Beta - message.Beta) < Tolerance &&
                                                                  Math.Abs(x.Gamma - message.Gamma) < Tolerance &&
                                                                  x.Iteration == message.Iteration &&
                                                                  x.Trail.SequenceEqual(message.Trail) &&
                                                                  x.Type == message.Type &&
                                                                  Math.Abs(x.Length - message.Length) < Tolerance));
        }
    }
}