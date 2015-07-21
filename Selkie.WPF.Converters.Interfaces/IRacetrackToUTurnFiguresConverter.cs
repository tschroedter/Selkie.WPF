using System.Windows.Media;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IRacetrackPathUTurnToFiguresConverter : IConverter
    {
        IPath Path { get; set; }
        PathFigureCollection FiguresCollection { get; }
    }
}