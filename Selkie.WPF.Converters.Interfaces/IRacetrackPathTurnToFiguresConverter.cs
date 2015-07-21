using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IRacetrackPathTurnToFiguresConverter : IConverter
    {
        [NotNull]
        PathFigureCollection FiguresCollection { get; }

        IPath Path { get; set; }
    }
}