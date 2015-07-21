using System.Collections.Generic;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;
using Point = System.Windows.Point;

namespace Selkie.WPF.Common.Converters
{
    public class LineToWindowPointsConverter : ILineToWindowPointsConverter
    {
        private readonly IGeometryPointToWindowsPointConverter m_Converter;
        private ILine m_Line = Geometry.Shapes.Line.Unknown;
        private Constants.LineDirection m_LineDirection = Constants.LineDirection.Forward;
        private IEnumerable <Point> m_Points = new List <Point>();

        public LineToWindowPointsConverter(IGeometryPointToWindowsPointConverter converter)
        {
            m_Converter = converter;
        }

        public ILine Line
        {
            get
            {
                return m_Line;
            }
            set
            {
                m_Line = value;
            }
        }

        public Constants.LineDirection LineDirection
        {
            get
            {
                return m_LineDirection;
            }
            set
            {
                m_LineDirection = value;
            }
        }

        public IEnumerable <Point> Points
        {
            get
            {
                return m_Points;
            }
        }

        public void Convert()
        {
            m_Points = CreatePointsForLine(Line,
                                           m_LineDirection);
        }

        internal IEnumerable <Point> CreatePointsForLine(ILine line,
                                                         Constants.LineDirection direction)
        {
            var points = new List <Point>();

            if ( direction == Constants.LineDirection.Forward )
            {
                IEnumerable <Point> forwardPoints = ConvertPoints(line.StartPoint,
                                                                  line.EndPoint);

                points.AddRange(forwardPoints);
            }
            else
            {
                IEnumerable <Point> reversePoints = ConvertPoints(line.EndPoint,
                                                                  line.StartPoint);

                points.AddRange(reversePoints);
            }

            return points;
        }

        internal IEnumerable <Point> ConvertPoints(Geometry.Shapes.Point startPoint,
                                                   Geometry.Shapes.Point endPoint)
        {
            var points = new List <Point>();

            m_Converter.GeometryPoint = startPoint;
            m_Converter.Convert();

            Point pointStart = m_Converter.Point;

            m_Converter.GeometryPoint = endPoint;
            m_Converter.Convert();

            Point pointEnd = m_Converter.Point;

            points.Add(pointStart);
            points.Add(pointEnd);

            return points;
        }
    }
}