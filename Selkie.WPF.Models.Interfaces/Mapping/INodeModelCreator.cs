using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Models.Interfaces.Mapping
{
    public interface INodeModelCreator
    {
        [NotNull]
        INodeIdHelper Helper { get; }

        [NotNull]
        INodeModel CreateNodeModel(int lineId,
                                   int nodeId);
    }
}