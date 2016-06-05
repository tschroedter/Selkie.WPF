using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels
{
    [ProjectComponent(Lifestyle.Singleton)]
    public sealed class MainViewModel : IMainViewModel
    {
        public MainViewModel(ICommandManager commandManager)
        {
            m_CommandManager = commandManager;
        }

        public IMainView ParentView { get; set; }
        private readonly ICommandManager m_CommandManager;
        private ICommand m_ClosingCommand;

        public ICommand ClosingCommand
        {
            get
            {
                return m_ClosingCommand ?? ( m_ClosingCommand = new DelegateCommand(m_CommandManager,
                                                                                    ClosingExecute,
                                                                                    ClosingCommandCanExecute) );
            }
        }

        [ExcludeFromCodeCoverage]
        public void ShutDown()
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        internal bool ClosingCommandCanExecute()
        {
            return true;
        }

        [ExcludeFromCodeCoverage]
        private void ClosingExecute()
        {
            ShutDown();
        }
    }
}