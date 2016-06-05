namespace Selkie.WPF.Models.Interfaces
{
    public interface IControlModel : IModel
    {
        bool IsRunning { get; }
        bool IsApplying { get; }
        void Apply();
        void Start();
        void Stop();
    }
}