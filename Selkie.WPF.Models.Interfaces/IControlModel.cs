namespace Selkie.WPF.Models.Interfaces
{
    public interface IControlModel : IModel
    {
        bool IsRunning { get; }
        bool IsApplying { get; }
        void Start();
        void Stop();
        void Apply();
    }
}