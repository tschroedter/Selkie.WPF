using System.Collections.Generic;

namespace Selkie.WPF.Models.Interfaces
{
    public interface IAntSettingsNodesManager
    {
        IEnumerable <IAntSettingsNode> Nodes { get; }
        void CreateNodesForCurrentLines();
    }
}