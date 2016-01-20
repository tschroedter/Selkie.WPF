using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common.Converters
{
    public class GeometryPointToWindowsPointConverter : IGeometryPointToWindowsPointConverter
    {
        internal static Point Origin = new Point(50.0,
                                                 1950.0);

        private Point m_GeometryPoint = Geometry.Shapes.Point.Unknown;

        private System.Windows.Point m_Point = new System.Windows.Point(double.MinValue,
                                                                        double.MinValue);

        private System.Windows.Point NewPointRelativeToOrigin(double x,
                                                              double y)
        {
            var newPoint = new System.Windows.Point(Origin.X + x,
                                                    Origin.Y + -y);

            return newPoint;
        }

        #region IGeometryPointToWindowsPointConverter Members

        public Point GeometryPoint
        {
            get
            {
                return m_GeometryPoint;
            }
            set
            {
                m_GeometryPoint = value;
            }
        }

        public System.Windows.Point Point
        {
            get
            {
                return m_Point;
            }
        }

        public void Convert()
        {
            m_Point = NewPointRelativeToOrigin(m_GeometryPoint.X,
                                               m_GeometryPoint.Y);
        }

        #endregion
    }
}