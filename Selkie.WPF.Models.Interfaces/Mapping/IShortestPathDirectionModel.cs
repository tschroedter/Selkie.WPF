using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Models.Interfaces.Mapping
{
    public interface IShortestPathDirectionModel : IModel
    {
        [NotNull]
        IEnumerable <INodeModel> Nodes { get; }
    }
}