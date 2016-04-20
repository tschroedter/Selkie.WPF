using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Status
{
    public class ExceptionThrownModel : IExceptionThrownModel
    {
        private readonly ISelkieInMemoryBus m_Bus;

        public ExceptionThrownModel([NotNull] ISelkieInMemoryBus bus)
        {
            m_Bus = bus;

            LastError = string.Empty;

            string subscriptionId = GetType().ToString();

            bus.SubscribeAsync <ColonyExceptionThrownMessage>(subscriptionId,
                                                              ColonyExceptionThrownHandler);

            bus.SubscribeAsync <ExceptionThrownClearErrorMessage>(subscriptionId,
                                                                  ClearExceptionThrownHandler);
        }

        [NotNull]
        public string LastError { get; set; }

        internal void ClearExceptionThrownHandler(ExceptionThrownClearErrorMessage obj)
        {
            LastError = string.Empty;

            SendMessage();
        }

        internal void ColonyExceptionThrownHandler([NotNull] ColonyExceptionThrownMessage message)
        {
            LastError = message.Text;

            SendMessage();
        }

        private void SendMessage()
        {
            var changedMessage = new ExceptionThrownChangedMessage
                                 {
                                     Text = LastError
                                 };

            m_Bus.PublishAsync(changedMessage);
        }
    }
}