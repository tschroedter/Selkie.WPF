using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [ProjectComponent(Lifestyle.Startable)]
    public class StatusHandler : SelkieMessageHandlerAsync <StatusMessage>
    {
        public StatusHandler([NotNull] ISelkieInMemoryBus inMemoryBus)
        {
            m_InMemoryBus = inMemoryBus;
        }

        private readonly ISelkieInMemoryBus m_InMemoryBus;

        public override void Handle(StatusMessage message)
        {
            m_InMemoryBus.PublishAsync(new ColonyStatusMessage
                                       {
                                           Text = message.Text
                                       });
        }
    }
}