namespace Selkie.WPF.Common.Interfaces
{
    public interface ITask
    {
        object UniqueId { get; }
        void Invoke();
    }
}