namespace Selkie.Framework.Interfaces.Aco
{
    public interface IServiceProxy
    {
        bool IsRunning { get; }
        bool IsColonyCreated { get; }
        bool IsFinished { get; }
        void CreateColony();
        bool Start();
        void Stop();
    }
}