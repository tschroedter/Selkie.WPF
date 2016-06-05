using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Surveying;

namespace Selkie.Framework.Converters
{
    public sealed class SurveyFeaturesToCostPerSurveyFeatureConverter : ISurveyFeaturesToCostPerSurveyFeatureConverter
    {
        public SurveyFeaturesToCostPerSurveyFeatureConverter()
        {
            Features = new ISurveyFeature[0];
            CostPerFeature = new int[0];
        }

        public void Convert()
        {
            CostPerFeature = CreateCostPerFeature(Features);
        }

        public IEnumerable <ISurveyFeature> Features { get; set; }

        public IEnumerable <int> CostPerFeature { get; private set; }

        internal int[] CreateCostPerFeature([NotNull] IEnumerable <ISurveyFeature> features)
        {
            ISurveyFeature[] array = features.ToArray();

            var costs = new int[array.Length * 2];

            var index = 0;

            foreach ( ISurveyFeature feature in array )
            {
                costs [ index++ ] = ( int ) feature.Length;
                costs [ index++ ] = ( int ) feature.Length;
            }

            return costs;
        }
    }
}