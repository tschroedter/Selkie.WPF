using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;

namespace Selkie.Framework.Services.Handlers
{
    public class ExceptionThrownHandler : SelkieMessageHandlerAsync <ExceptionThrownMessage>
    {
        public ExceptionThrownHandler([NotNull] ISelkieInMemoryBus inMemoryBus)
        {
            m_InMemoryBus = inMemoryBus;
        }

        private readonly ISelkieInMemoryBus m_InMemoryBus;

        public override void Handle(ExceptionThrownMessage message)
        {
            m_InMemoryBus.PublishAsync(new ColonyExceptionThrownMessage
                                       {
                                           Text = message.Exception.Message
                                       });
        }
    }
}