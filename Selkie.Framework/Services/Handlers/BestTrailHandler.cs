using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [ProjectComponent(Lifestyle.Startable)]
    public class BestTrailHandler : SelkieMessageConsumer <BestTrailMessage>
    {
        private readonly IBus m_Bus;

        public BestTrailHandler([NotNull] IBus bus)
        {
            m_Bus = bus;
        }

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