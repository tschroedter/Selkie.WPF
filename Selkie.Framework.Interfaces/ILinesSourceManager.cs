using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;

namespace Selkie.Framework.Interfaces
{
    public interface ILinesSourceManager
    {
        [NotNull]
        IEnumerable <ILine> Lines { get; }

        [NotNull]
        IEnumerable <int> CostPerFeature { get; }

        [NotNull]
        IEnumerable <ISurveyPolyline> SurveyPolylines { get; }

        [NotNull]
        IEnumerable <ISurveyFeature> SurveyFeatures { get; }
    }
}