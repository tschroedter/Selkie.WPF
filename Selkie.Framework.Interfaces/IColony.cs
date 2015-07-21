namespace Selkie.Framework.Interfaces
{
    public interface IColony
    {
        bool IsRunning { get; }
        int SleepTimeInMs { get; }
    }
}