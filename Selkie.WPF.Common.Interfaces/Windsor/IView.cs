namespace Selkie.WPF.Common.Interfaces.Windsor
{
    public interface IView
    {
        object GetContent();
        void Show();
        bool? ShowDialog();
    }
}