using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [Interceptor(typeof( StatusAspect ))]
    [ProjectComponent(Lifestyle.Startable)]
    public class BestTrailHandler : SelkieMessageHandlerAsync <BestTrailMessage>
    {
        public BestTrailHandler([NotNull] ISelkieInMemoryBus bus)
        {
            m_Bus = bus;
        }

        private readonly ISelkieInMemoryBus m_Bus;

        [Status("Colony found a new best trail!")]
        public override void Handle(BestTrailMessage message)
        {
            m_Bus.PublishAsync(new ColonyBestTrailMessage
                               {
                                   Alpha = message.Alpha,
                                   Beta = message.Beta,
                                   Gamma = message.Gamma,
                                   Iteration = message.Iteration,
                                   Trail = message.Trail,
                                   Type = message.Type,
                                   Length = message.Length
                               });
        }
    }
}