using System.Linq;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Converters;
using Point = System.Windows.Point;

namespace Selkie.WPF.Common
{
    [ProjectComponent(Lifestyle.Transient)]
    public class DisplayLine : IDisplayLine
    {
        private readonly double m_DirectionAngle;
        private readonly Point m_EndPoint;
        private readonly int m_Id;
        private readonly string m_Name;
        private readonly Point m_StartPoint;

        public DisplayLine(ILineToWindowPointsConverter converter,
                           ILine line)
        {
            converter.Line = line;
            converter.Convert();

            Point[] points = converter.Points.ToArray();

            m_Id = line.Id;
            m_StartPoint = points.First();
            m_EndPoint = points.Last();
            m_DirectionAngle = CalculateAngle(m_StartPoint,
                                              m_EndPoint,
                                              line.RunDirection);

            string position = string.Format(" [{0},{1} - {2},{3}] @{4:F2}deg",
                                            X1,
                                            Y1,
                                            X2,
                                            Y2,
                                            m_DirectionAngle);

            m_Name = "Line " + m_Id + position;
        }

        internal double CalculateAngle(Point startPoint,
                                       Point endPoint,
                                       Constants.LineDirection direction)
        {
            var geoStartPoint = new Geometry.Shapes.Point(startPoint.X,
                                                          startPoint.Y);
            var geoEndPoint = new Geometry.Shapes.Point(endPoint.X,
                                                        endPoint.Y);

            var line = new Line(geoStartPoint,
                                geoEndPoint,
                                direction);

            return line.AngleToXAxis.Degrees;
        }

        #region IDisplayLine Members

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public double X1
        {
            get
            {
                return m_StartPoint.X;
            }
        }

        public double Y1
        {
            get
            {
                return m_StartPoint.Y;
            }
        }

        public double X2
        {
            get
            {
                return m_EndPoint.X;
            }
        }

        public double Y2
        {
            get
            {
                return m_EndPoint.Y;
            }
        }

        public Point StartPoint
        {
            get
            {
                return m_StartPoint;
            }
        }

        public Point EndPoint
        {
            get
            {
                return m_EndPoint;
            }
        }

        public double DirectionAngle
        {
            get
            {
                return m_DirectionAngle;
            }
        }

        #endregion
    }
}