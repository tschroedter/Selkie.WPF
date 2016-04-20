using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Control
{
    public class ControlViewModel
        : ViewModel,
          IControlViewModel
    {
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ICommandManager m_CommandManager;
        private readonly IControlModel m_ControlModel;
        private readonly IApplicationDispatcher m_Dispatcher;
        private ICommand m_ApplyCommand;
        private ICommand m_ExitCommand;
        private string m_SelectedTestLine = string.Empty;
        private ICommand m_StartCommand;
        private ICommand m_StopCommand;
        private IEnumerable <string> m_TestLines = new string[0];

        public ControlViewModel([NotNull] ISelkieInMemoryBus bus,
                                [NotNull] IApplicationDispatcher dispatcher,
                                [NotNull] IControlModel controlModel,
                                [NotNull] ICommandManager commandManager)
        {
            m_Bus = bus;
            m_Dispatcher = dispatcher;
            m_ControlModel = controlModel;
            m_CommandManager = commandManager;

            bus.SubscribeAsync <ControlModelChangedMessage>(GetType().ToString(),
                                                            ControlModelChangedHandler);

            bus.SubscribeAsync <ControlModelTestLinesResponseMessage>(GetType().ToString(),
                                                                      ControlModelTestLinesResponseHandler);

            bus.PublishAsync(new ControlModelTestLinesRequestMessage());
        }

        public IView Parent { get; set; }

        public ICommand ApplyCommand
        {
            get
            {
                return m_ApplyCommand ?? ( m_ApplyCommand = new DelegateCommand(m_CommandManager,
                                                                                m_ControlModel.Apply,
                                                                                CanExecuteApplyCommand) );
            }
        }

        public bool IsApplyEnabled
        {
            get
            {
                return CanExecuteApplyCommand();
            }
        }

        internal bool IsRunning { get; private set; }
        internal bool IsApplying { get; private set; }

        public bool IsLinesWereApplied { get; private set; }

        public ICommand StopCommand
        {
            get
            {
                return m_StopCommand ?? ( m_StopCommand = new DelegateCommand(m_CommandManager,
                                                                              m_ControlModel.Stop,
                                                                              CanExecuteStopCommand) );
            }
        }

        public bool IsStopEnabled
        {
            get
            {
                return CanExecuteStopCommand();
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return m_ExitCommand ?? ( m_ExitCommand = new DelegateCommand(m_CommandManager,
                                                                              ExitExecute,
                                                                              CanExecuteExitCommand) );
            }
        }

        public bool IsExitEnabled
        {
            get
            {
                return CanExecuteExitCommand();
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return m_StartCommand ?? ( m_StartCommand = new DelegateCommand(m_CommandManager,
                                                                                m_ControlModel.Start,
                                                                                CanExecuteStartCommand) );
            }
        }

        public bool IsStartEnabled
        {
            get
            {
                return CanExecuteStartCommand();
            }
        }

        public IEnumerable <string> TestLines
        {
            get
            {
                return m_TestLines;
            }
        }

        public string SelectedTestLine
        {
            get
            {
                return m_SelectedTestLine;
            }
            set
            {
                m_SelectedTestLine = value;

                IsLinesWereApplied = false;

                m_Bus.PublishAsync(new ControlModelTestLineSetMessage
                                   {
                                       Type = m_SelectedTestLine
                                   });

                m_Dispatcher.BeginInvoke(Update);
            }
        }

        internal void ControlModelChangedHandler(ControlModelChangedMessage message)
        {
            IsRunning = message.IsRunning;
            IsApplying = message.IsApplying;

            IsLinesWereApplied = !string.IsNullOrEmpty(SelectedTestLine);
            // todo a bit of a hack here we really don't know

            m_Dispatcher.BeginInvoke(Update);
        }

        internal void Update()
        {
            NotifyPropertyChanged("IsStartEnabled");
            NotifyPropertyChanged("IsStopEnabled");
            NotifyPropertyChanged("IsApplyEnabled");
            NotifyPropertyChanged("TestLines");
            NotifyPropertyChanged("SelectedTestLines");

            m_CommandManager.InvalidateRequerySuggested();
        }

        //ncrunch: no coverage start
        [ExcludeFromCodeCoverage]
        private void ExitExecute()
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        //ncrunch: no coverage end

        internal bool CanExecuteApplyCommand()
        {
            return !string.IsNullOrEmpty(m_SelectedTestLine) &&
                   !IsRunning &&
                   !IsApplying;
        }

        internal bool CanExecuteStartCommand()
        {
            return !IsRunning &&
                   IsLinesWereApplied;
        }

        internal bool CanExecuteStopCommand()
        {
            return IsRunning;
        }

        internal bool CanExecuteExitCommand()
        {
            return true;
        }

        internal void ControlModelTestLinesResponseHandler(ControlModelTestLinesResponseMessage message)
        {
            m_TestLines = message.TestLineTypes;

            m_Dispatcher.BeginInvoke(Update);
        }
    }
}