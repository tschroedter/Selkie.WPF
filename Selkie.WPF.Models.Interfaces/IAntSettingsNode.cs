using JetBrains.Annotations;

namespace Selkie.WPF.Models.Interfaces
{
    public interface IAntSettingsNode
    {
        int Id { get; }

        [NotNull]
        string Description { get; }
    }
}