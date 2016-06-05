using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Surveying;

namespace Selkie.Framework.Interfaces.Converters
{
    public interface ISurveyFeaturesToCostPerSurveyFeatureConverter : IConverter
    {
        [NotNull]
        IEnumerable <ISurveyFeature> Features { get; set; }

        [NotNull]
        IEnumerable <int> CostPerFeature { get; }
    }
}