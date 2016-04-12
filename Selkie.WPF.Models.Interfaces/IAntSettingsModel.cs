using System.Collections.Generic;
using JetBrains.Annotations;

namespace Selkie.WPF.Models.Interfaces
{
    public interface IAntSettingsModel : IModel
    {
        bool IsFixedStartNode { get; }
        int FixedStartNode { get; }

        [NotNull]
        IEnumerable <IAntSettingsNode> Nodes { get; }
    }
}