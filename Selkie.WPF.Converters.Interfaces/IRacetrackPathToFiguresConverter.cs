using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IRacetrackPathToFiguresConverter : IConverter
    {
        [NotNull]
        IPath Path { get; set; }

        [NotNull]
        PathFigureCollection FiguresCollection { get; }
    }
}