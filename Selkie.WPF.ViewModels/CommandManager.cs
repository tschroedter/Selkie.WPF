using System;
using System.Diagnostics.CodeAnalysis;
using Selkie.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels
{
    //ncrunch: no coverage start
    [ExcludeFromCodeCoverage]
    [ProjectComponent(Lifestyle.Singleton)]
    public class CommandManager : ICommandManager
    {
        public void InvalidateRequerySuggested()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler RequerySuggested
        {
            add
            {
                System.Windows.Input.CommandManager.RequerySuggested += value;
            }
            remove
            {
                System.Windows.Input.CommandManager.RequerySuggested -= value;
            }
        }
    }
}