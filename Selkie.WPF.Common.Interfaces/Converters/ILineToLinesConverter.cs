using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Interfaces.Converters
{
    public interface ILineToLinesConverter : IConverter
    {
        ILine Line { get; set; }
        IEnumerable <ILine> Lines { get; set; }
        double BaseCost { get; }

        [NotNull]
        IRacetracks Racetracks { get; set; }

        double CostEndToEnd([NotNull] ILine other);
        double CostEndToStart([NotNull] ILine other);

        double CostForwardForward([NotNull] ILine other);
        double CostForwardReverse([NotNull] ILine other);
        double CostReverseForward([NotNull] ILine to);
        double CostReverseReverse([NotNull] ILine to);
        double CostStartToEnd([NotNull] ILine to);
        double CostStartToStart([NotNull] ILine other);
    }
}