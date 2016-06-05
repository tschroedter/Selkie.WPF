using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;

namespace Selkie.Framework.Interfaces
{
    public interface ISurveyFeatureSource
    {
        [NotNull]
        IEnumerable <ILine> Lines { get; }

        [NotNull]
        IEnumerable <int> CostPerFeature { get; }

        [NotNull]
        IEnumerable <ISurveyPolyline> SurveyPolylines { get; }
    }
}