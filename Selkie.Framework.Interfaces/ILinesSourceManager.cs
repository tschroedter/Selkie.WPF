using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Interfaces
{
    public interface ILinesSourceManager
    {
        [NotNull]
        IEnumerable <ILine> Lines { get; }

        [NotNull]
        IEnumerable <int> CostPerLine { get; }
    }
}