using System;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface ICommandManager
    {
        void InvalidateRequerySuggested();
        event EventHandler RequerySuggested;
    }
}