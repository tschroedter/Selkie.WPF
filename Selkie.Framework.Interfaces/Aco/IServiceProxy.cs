using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces.Aco
{
    public interface IServiceProxy
    {
        bool IsRunning { get; }
        bool IsColonyCreated { get; }
        bool IsFinished { get; }
        void CreateColony([NotNull] IColonyParameters colonyParameters);
        bool Start();
        void Stop();
    }
}