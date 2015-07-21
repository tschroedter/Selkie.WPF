using System;
using System.Windows.Threading;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IApplicationDispatcher
    {
        void BeginInvoke(Action action);

        void Schedule(DispatcherPriority priority,
                      Action action);
    }
}