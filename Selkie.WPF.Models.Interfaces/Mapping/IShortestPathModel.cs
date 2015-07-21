using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Models.Interfaces.Mapping
{
    public interface IShortestPathModel : IModel
    {
        [NotNull]
        IEnumerable <ILineToLineNodeConverter> Nodes { get; }

        [NotNull]
        IEnumerable <IDisplayLine> Path { get; }
    }
}