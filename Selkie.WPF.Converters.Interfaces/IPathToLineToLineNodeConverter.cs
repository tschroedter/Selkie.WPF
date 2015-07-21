using System.Collections.Generic;
using JetBrains.Annotations;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IPathToLineToLineNodeConverter : IConverter
    {
        IEnumerable <ILineToLineNodeConverter> Nodes { get; }

        [NotNull]
        IEnumerable <int> Path { get; set; }
    }
}