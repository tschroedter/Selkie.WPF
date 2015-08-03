using System.Collections.Generic;
using System.Linq;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Services.Lines.Common.Dto;

namespace Selkie.Framework.Converters
{
    public class TestLinesDtoToLinesConverter : ITestLinesDtoToLinesConverter
    {
        private IEnumerable <LineDto> m_Dtos = new LineDto[0];
        private IEnumerable <ILine> m_Lines = new ILine[0];

        public IEnumerable <LineDto> Dtos
        {
            get
            {
                return m_Dtos;
            }
            set
            {
                m_Dtos = value;
            }
        }

        public IEnumerable <ILine> Lines
        {
            get
            {
                return m_Lines;
            }
        }

        public void Convert()
        {
            m_Lines = m_Dtos.Select(CreateLine);
        }

        internal ILine CreateLine(LineDto lineDto)
        {
            Constants.LineDirection runDirection = ConvertStringToLineDirection(lineDto.RunDirection);

            var line = new Line(lineDto.Id,
                                lineDto.X1,
                                lineDto.Y1,
                                lineDto.X2,
                                lineDto.Y2,
                                lineDto.IsUnknown,
                                runDirection);
            return line;
        }

        private Constants.LineDirection ConvertStringToLineDirection(string runDirection)
        {
            if ( Constants.LineDirection.Forward.ToString() == runDirection )
            {
                return Constants.LineDirection.Forward;
            }

            if ( Constants.LineDirection.Reverse.ToString() == runDirection )
            {
                return Constants.LineDirection.Reverse;
            }

            return Constants.LineDirection.Unknown;
        }
    }
}