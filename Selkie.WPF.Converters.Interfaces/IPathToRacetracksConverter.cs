using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IPathToRacetracksConverter : IConverter
    {
        [NotNull]
        IEnumerable <int> Path { get; set; }

        [NotNull]
        IEnumerable <IPath> Paths { get; }
    }
}