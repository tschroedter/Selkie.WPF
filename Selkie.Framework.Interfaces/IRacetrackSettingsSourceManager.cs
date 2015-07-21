using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces
{
    public interface IRacetrackSettingsSourceManager
    {
        [NotNull]
        IRacetrackSettingsSource Source { get; }
    }
}