using System.Windows.Input;
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
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ICommandManager m_CommandManager;
        private readonly IApplicationDispatcher m_Dispatcher;
        private ICommand m_ClearErrorCommand;

        public StatusViewModel([NotNull] ISelkieInMemoryBus bus,
                               [NotNull] IApplicationDispatcher dispatcher,
                               [NotNull] ICommandManager commandManager,
                               [UsedImplicitly] IExceptionThrownModel exceptionThrownModel,
                               [UsedImplicitly] IStatusModel statusModel)
        {
            m_Bus = bus;
            m_Dispatcher = dispatcher;
            m_CommandManager = commandManager;

            Status = string.Empty;
            ExceptionThrown = string.Empty;

            bus.SubscribeAsync <StatusChangedMessage>(GetType().ToString(),
                                                      StatusChangedHandler);

            bus.SubscribeAsync <ExceptionThrownChangedMessage>(GetType().ToString(),
                                                               ExceptionThrownChangedHandler);
        }

        public ICommand ClearErrorCommand
        {
            get
            {
                return m_ClearErrorCommand ?? ( m_ClearErrorCommand = new DelegateCommand(m_CommandManager,
                                                                                          SendClearErrorMessage,
                                                                                          CanExecuteClearErrorCommand) );
            }
        }

        public bool IsClearErrorEnabled
        {
            get
            {
                return CanExecuteClearErrorCommand();
            }
        }

        public string ExceptionThrown { get; private set; }

        public string Status { get; private set; }

        internal void SendClearErrorMessage()
        {
            m_Bus.PublishAsync(new ExceptionThrownClearErrorMessage());
        }

        internal bool CanExecuteClearErrorCommand()
        {
            return !string.IsNullOrEmpty(ExceptionThrown);
        }

        internal void StatusChangedHandler(StatusChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        internal void ExceptionThrownChangedHandler(ExceptionThrownChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        private void UpdateAndNotify(StatusChangedMessage message)
        {
            Status = message.Text ?? string.Empty;

            NotifyPropertyChanged("Status");
        }

        private void UpdateAndNotify(ExceptionThrownChangedMessage message)
        {
            ExceptionThrown = message.Text ?? string.Empty;

            NotifyPropertyChanged("ExceptionThrown");
            NotifyPropertyChanged("IsClearErrorEnabled");

            m_CommandManager.InvalidateRequerySuggested();
        }
    }
}