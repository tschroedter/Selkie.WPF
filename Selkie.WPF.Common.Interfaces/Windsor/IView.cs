namespace Selkie.WPF.Common.Interfaces.Windsor
{
    public interface IView
    {
        void Show();
        bool? ShowDialog();
        object GetContent();
    }
}