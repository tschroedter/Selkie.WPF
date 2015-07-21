using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.Framework.Common.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [ProjectComponent(Lifestyle.Startable)]
    public class PheromonesHandler : BaseHandler <PheromonesMessage>
    {
        public PheromonesHandler([NotNull] ILogger logger,
                                 [NotNull] IBus bus)
            : base(logger,
                   bus)
        {
        }

        internal override void Handle(PheromonesMessage message)
        {
            var colonyPheromonesMessage = new ColonyPheromonesMessage
                                          {
                                              Values = message.Values,
                                              Minimum = message.Minimum,
                                              Maximum = message.Maximum,
                                              Average = message.Average
                                          };

            Bus.PublishAsync(colonyPheromonesMessage);
        }
    }
}