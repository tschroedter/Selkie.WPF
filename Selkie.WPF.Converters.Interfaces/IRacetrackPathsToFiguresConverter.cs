using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IRacetrackPathsToFiguresConverter : IConverter
    {
        [NotNull]
        IEnumerable <PathFigureCollection> Figures { get; }

        [NotNull]
        IEnumerable <IPath> Paths { get; set; }
    }
}