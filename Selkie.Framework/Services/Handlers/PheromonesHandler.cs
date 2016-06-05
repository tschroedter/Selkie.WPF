using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [ProjectComponent(Lifestyle.Startable)]
    public class PheromonesHandler : SelkieMessageHandlerAsync <PheromonesMessage>
    {
        public PheromonesHandler([NotNull] ISelkieInMemoryBus bus)
        {
            m_Bus = bus;
        }

        private readonly ISelkieInMemoryBus m_Bus;

        public override void Handle(PheromonesMessage message)
        {
            var colonyPheromonesMessage = new ColonyPheromonesMessage
                                          {
                                              Values = message.Values,
                                              Minimum = message.Minimum,
                                              Maximum = message.Maximum,
                                              Average = message.Average
                                          };

            m_Bus.PublishAsync(colonyPheromonesMessage);
        }
    }
}