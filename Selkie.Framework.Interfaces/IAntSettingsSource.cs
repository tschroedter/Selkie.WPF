namespace Selkie.Framework.Interfaces
{
    public interface IAntSettingsSource
    {
        bool IsFixedStartNode { get; }
        int FixedStartNode { get; }
    }
}