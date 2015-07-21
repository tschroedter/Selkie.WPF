using System.Windows;
using System.Windows.Media;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common
{
    [ProjectComponent(Lifestyle.Transient)]
    public class DisplayNode : IDisplayNode
    {
        public const int UnknownId = -1;
        internal const double DefaultRadius = 5.0;
        internal const double DefaultStrokeThickness = 1.0;
        public static IDisplayNode Unknown = new DisplayNode();
        internal static readonly SolidColorBrush DefaultFill = Brushes.DodgerBlue;
        internal static readonly SolidColorBrush DefaultStroke = Brushes.Black;
        private readonly Point m_CentrePoint;
        private readonly double m_DirectionAngle;
        private readonly SolidColorBrush m_FillValue;
        private readonly int m_Id;
        private readonly string m_Name;
        private readonly Point m_OriginalCentrePoint;
        private readonly Point m_Point;
        private readonly double m_Radius;
        private readonly double m_StrokeThicknessValue;
        private readonly SolidColorBrush m_StrokeValue;

        private DisplayNode()
        {
            m_Id = UnknownId;
        }

        public DisplayNode(IGeometryPointToWindowsPointConverter converter,
                           int id,
                           double x,
                           double y,
                           double directionAngle,
                           double radius,
                           SolidColorBrush fill,
                           SolidColorBrush stroke,
                           double strokeThickness)
        {
            m_Id = id;
            m_Radius = radius;
            m_FillValue = fill;
            m_StrokeValue = stroke;
            m_StrokeThicknessValue = strokeThickness;
            m_DirectionAngle = directionAngle;
            m_OriginalCentrePoint = new Point(x,
                                              y);
            converter.GeometryPoint = new Geometry.Shapes.Point(x,
                                                                y);
            converter.Convert();
            m_CentrePoint = converter.Point;
            m_Point = new Point(m_CentrePoint.X - m_Radius,
                                m_CentrePoint.Y - m_Radius);
            m_Name = "Node " + m_Id;
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public Point OriginalCentrePoint
        {
            get
            {
                return m_OriginalCentrePoint;
            }
        }

        public Point CentrePoint
        {
            get
            {
                return m_CentrePoint;
            }
        }

        public Point Position
        {
            get
            {
                return m_Point;
            }
        }

        public double X
        {
            get
            {
                return m_Point.X;
            }
        }

        public double Y
        {
            get
            {
                return m_Point.Y;
            }
        }

        public double Width
        {
            get
            {
                return m_Radius * 2.0;
            }
        }

        public double Height
        {
            get
            {
                return m_Radius * 2.0;
            }
        }

        public double Radius
        {
            get
            {
                return m_Radius;
            }
        }

        public SolidColorBrush Stroke
        {
            get
            {
                return m_StrokeValue;
            }
        }

        public SolidColorBrush Fill
        {
            get
            {
                return m_FillValue;
            }
        }

        public double StrokeThickness
        {
            get
            {
                return m_StrokeThicknessValue;
            }
        }

        public double DirectionAngle
        {
            get
            {
                return m_DirectionAngle;
            }
        }

        public bool IsUnknown
        {
            get
            {
                return m_Id == UnknownId;
            }
        }

        public bool IsVisible
        {
            get
            {
                return !IsUnknown;
            }
        }
    }
}