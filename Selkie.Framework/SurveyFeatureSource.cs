using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class SurveyFeatureSource : ISurveyFeatureSource
    {
        private SurveyFeatureSource()
        {
            CostPerFeature = new int[0];
            Lines = new ILine[0];
            SurveyPolylines = new ISurveyPolyline[0];
        }

        public SurveyFeatureSource([NotNull] ISurveyFeaturesToCostPerSurveyFeatureConverter converter,
                                   [NotNull] IEnumerable <ILine> lines)
        {
            Lines = lines.ToArray();
            SurveyPolylines = Lines.Select(CreateSurveyPolylineFromLine);

            converter.Features = SurveyPolylines;
            converter.Convert();

            CostPerFeature = converter.CostPerFeature;
        }

        public static ISurveyFeatureSource Unknown = new SurveyFeatureSource();

        public IEnumerable <ILine> Lines { get; private set; } // todo should be replaced by SurveyPolylines

        public IEnumerable <ISurveyPolyline> SurveyPolylines { get; private set; }

        public IEnumerable <int> CostPerFeature { get; private set; }

        private static SurveyPolyline CreateSurveyPolylineFromLine(ILine line)
        {
            IPolyline polyline = new Polyline(line.Id,
                                              line.RunDirection);

            polyline.AddSegment(line);

            return new SurveyPolyline(polyline);
        }
    }
}