using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Status
{
    public class StatusViewModel
        : ViewModel,
          IStatusViewModel
    {
        private readonly IApplicationDispatcher m_Dispatcher;

        public StatusViewModel([NotNull] ISelkieInMemoryBus bus,
                               [NotNull] IApplicationDispatcher dispatcher,
                               [UsedImplicitly] IStatusModel model)
        {
            m_Dispatcher = dispatcher;

            Status = string.Empty;

            bus.SubscribeAsync <StatusChangedMessage>(GetType().ToString(),
                                                      StatusChangedHandler);
        }

        public string Status { get; private set; }

        internal void StatusChangedHandler(StatusChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        private void UpdateAndNotify(StatusChangedMessage message)
        {
            Status = message.Text ?? string.Empty;

            NotifyPropertyChanged("Status");
        }
    }
}