using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Common
{
    // todo check latest in IOC/WPF solution
    [ExcludeFromCodeCoverage]
    [ProjectComponent(Lifestyle.Transient)]
    public class ApplicationDispatcher : IApplicationDispatcher
    {
        private Dispatcher m_Dispatcher;

        public void BeginInvoke(Action action)
        {
            Dispatcher dispatcher = GetDispatcher();

            Action actionTask = () => Execute(action);

            if ( dispatcher == null ||
                 dispatcher.CheckAccess() )
            {
                actionTask();
            }
            else
            {
                dispatcher.BeginInvoke(actionTask);
            }
        }

        public void Schedule(DispatcherPriority priority,
                             Action action)
        {
            Dispatcher dispatcher = GetDispatcher();

            Action actionTask = () => Execute(action);

            if ( dispatcher == null )
            {
                actionTask();
            }
            else
            {
                dispatcher.BeginInvoke(actionTask,
                                       priority);
            }
        }

        private void Execute(Action action)
        {
            action();

            // todo First!!!!
            // TaskRunner.RunTask(new DispatcherTask(action));
        }

        private Dispatcher GetDispatcher()
        {
            if ( m_Dispatcher != null )
            {
                return m_Dispatcher;
            }

            Application application = Application.Current;

            if ( application != null )
            {
                m_Dispatcher = application.Dispatcher;
            }

            return m_Dispatcher;
        }
    }
}