using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Surveying;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Interfaces.Converters
{
    public interface ISurveyFeatureToSurveyFeatureDtoConverter : IConverter
    {
        [NotNull]
        IEnumerable <ISurveyFeature> Features { get; set; }

        [NotNull]
        IEnumerable <SurveyFeatureDto> Dtos { get; }
    }
}