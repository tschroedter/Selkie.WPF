using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Point = System.Windows.Point;

namespace Selkie.WPF.Common.Interfaces.Converters
{
    public interface ILineToWindowPointsConverter : IConverter
    {
        [NotNull]
        ILine Line { get; set; }

        Constants.LineDirection LineDirection { get; set; }

        [NotNull]
        IEnumerable <Point> Points { get; }
    }
}