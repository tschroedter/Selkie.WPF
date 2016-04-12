using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces.Aco
{
    public interface IServiceProxy
    {
        bool IsRunning { get; }
        bool IsColonyCreated { get; }
        bool IsFinished { get; }
        bool Start();
        void Stop();
        void CreateColony([NotNull] IColonyParameters colonyParameters);
    }
}