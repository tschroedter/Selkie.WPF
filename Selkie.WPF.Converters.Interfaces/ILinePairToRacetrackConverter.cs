using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface ILinePairToRacetrackConverter : IConverter
    {
        [NotNull]
        ILine FromLine { get; set; }

        [NotNull]
        ILine ToLine { get; set; }

        double Radius { get; set; }

        [CanBeNull]
        IRacetrackSettingsSource Settings { get; }

        [NotNull]
        IPath Racetrack { get; }
    }
}