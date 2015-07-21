using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface ILineToLineNodeConverter : IConverter
    {
        [NotNull]
        ILine From { get; set; }

        Constants.LineDirection FromDirection { get; set; }

        [NotNull]
        ILine To { get; set; }

        Constants.LineDirection ToDirection { get; set; }
        double Cost { get; }

        [NotNull]
        IEnumerable <Point> Points { get; }
    }
}