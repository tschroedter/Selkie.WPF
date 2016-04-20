using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Status
{
    public class StatusModel : IStatusModel
    {
        private readonly ISelkieInMemoryBus m_Bus;

        public StatusModel([NotNull] ISelkieInMemoryBus bus)
        {
            m_Bus = bus;

            string subscriptionId = GetType().ToString();

            bus.SubscribeAsync <ColonyStatusMessage>(subscriptionId,
                                                     StatusHandler);
        }

        internal void StatusHandler(ColonyStatusMessage message)
        {
            var changedMessage = new StatusChangedMessage
                                 {
                                     Text = message.Text
                                 };

            m_Bus.PublishAsync(changedMessage);
        }
    }
}