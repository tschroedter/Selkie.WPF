using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;
using Selkie.Services.Lines.Common.Dto;

namespace Selkie.Framework.Converter
{
    public interface ITestLinesDtoToLinesConverter : IConverter
    {
        [NotNull]
        IEnumerable <LineDto> Dtos { get; set; }

        [NotNull]
        IEnumerable <ILine> Lines { get; }
    }
}