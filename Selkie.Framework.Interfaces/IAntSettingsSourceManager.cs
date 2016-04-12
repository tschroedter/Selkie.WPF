using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces
{
    public interface IAntSettingsSourceManager
    {
        [NotNull]
        IAntSettingsSource Source { get; }
    }
}