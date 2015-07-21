using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Models.Interfaces.Mapping
{
    public interface IStartNodeModel : IModel
    {
        [NotNull]
        INodeModel Node { get; }
    }
}