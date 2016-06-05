using System.Collections.Generic;
using System.Linq;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Surveying;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Converters
{
    public class SurveyFeatureToSurveyFeatureDtoConverter : ISurveyFeatureToSurveyFeatureDtoConverter
    {
        public SurveyFeatureToSurveyFeatureDtoConverter()
        {
            Features = new ISurveyFeature[0];
            Dtos = new SurveyFeatureDto[0];
        }

        public IEnumerable <ISurveyFeature> Features { get; set; }
        public IEnumerable <SurveyFeatureDto> Dtos { get; private set; }

        public void Convert()
        {
            IEnumerable <SurveyFeatureDto> dtos = Features.Select(feature => new SurveyFeatureDto
                                                                             {
                                                                                 EndPoint = new PointDto
                                                                                            {
                                                                                                X = feature.EndPoint.X,
                                                                                                Y = feature.EndPoint.Y
                                                                                            },
                                                                                 RunDirection =
                                                                                     feature.RunDirection.ToString(),
                                                                                 StartPoint = new PointDto
                                                                                              {
                                                                                                  X =
                                                                                                      feature.StartPoint
                                                                                                             .X,
                                                                                                  Y =
                                                                                                      feature.StartPoint
                                                                                                             .Y
                                                                                              },
                                                                                 AngleToXAxisAtEndPoint =
                                                                                     feature.AngleToXAxisAtEndPoint
                                                                                            .Degrees,
                                                                                 AngleToXAxisAtStartPoint =
                                                                                     feature.AngleToXAxisAtStartPoint
                                                                                            .Degrees,
                                                                                 Id = feature.Id,
                                                                                 IsUnknown = feature.IsUnknown,
                                                                                 Length = feature.Length
                                                                             });

            Dtos = dtos;
        }
    }
}