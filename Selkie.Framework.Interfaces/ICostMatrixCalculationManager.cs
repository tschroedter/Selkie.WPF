using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Interfaces
{
    public interface ICostMatrixCalculationManager
    {
        bool IsCalculating { get; }
        bool IsReceivedColonyLinesChangedMessage { get; }
        bool IsReceivedColonyRacetrackSettingsChangedMessage { get; }

        [NotNull]
        int[][] Matrix { get; }

        [NotNull]
        IEnumerable <ILine> Lines { get; }

        [NotNull]
        IEnumerable <int> CostPerLine { get; }

        void Calculate();
    }
}