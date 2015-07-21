using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Interfaces.Converters
{
    public interface ILinesToCostPerLineConverter : IConverter
    {
        [NotNull]
        IEnumerable <ILine> Lines { get; set; }

        [NotNull]
        IEnumerable <int> CostPerLine { get; }
    }
}