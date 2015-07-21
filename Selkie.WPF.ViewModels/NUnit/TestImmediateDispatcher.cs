using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.ViewModels.NUnit
{
    //ncrunch: no coverage start
    [ExcludeFromCodeCoverage]
    internal sealed class TestImmediateDispatcher : IApplicationDispatcher
    {
        public void BeginInvoke(Action action)
        {
            action();
        }

        public void Schedule(DispatcherPriority priority,
                             Action action)
        {
            action();
        }
    }
}