using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Surveying;

namespace Selkie.Framework.Interfaces
{
    public interface ICostMatrixCalculationManager
    {
        [NotNull]
        int[][] Matrix { get; }

        [NotNull]
        IEnumerable <int> CostPerFeature { get; }

        [NotNull]
        ISurveyFeature[] SurveyFeature { get; }

        void Calculate();
    }
}