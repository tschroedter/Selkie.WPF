using System.Windows;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Views
{
    public class BaseView
        : Window,
          IView
    {
        public object GetContent()
        {
            return Content;
        }
    }
}