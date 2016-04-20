using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Interfaces
{
    public interface ICostMatrixCalculationManager
    {
        [NotNull]
        int[][] Matrix { get; }

        [NotNull]
        IEnumerable <ILine> Lines { get; }

        [NotNull]
        IEnumerable <LineDto> LineDtos { get; }

        [NotNull]
        IEnumerable <int> CostPerLine { get; }

        void Calculate();
    }
}