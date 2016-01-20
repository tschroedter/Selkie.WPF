using System.Windows;
using System.Windows.Media;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces.Converters;
using Selkie.WPF.Converters.Interfaces;
using ArcSegment = System.Windows.Media.ArcSegment;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters
{
    [ProjectComponent(Lifestyle.Transient)]
    public class PathSegmentHelper : IPathSegmentHelper
    {
        private readonly IGeometryPointToWindowsPointConverter m_Converter;

        public PathSegmentHelper(IGeometryPointToWindowsPointConverter converter)
        {
            m_Converter = converter;
        }

        public LineSegment SegmentToLineSegment(ILine segment)
        {
            m_Converter.GeometryPoint = segment.EndPoint;
            m_Converter.Convert();

            var line = new LineSegment(m_Converter.Point,
                                       true);

            return line;
        }

        public ArcSegment SegmentToArcSegment(ITurnCircleArcSegment segment)
        {
            Geometry.Shapes.Point endPoint = segment.EndPoint;

            Point finishEndPoint = PointRelativeToOrigin(endPoint);
            var finishSize = new Size(segment.Radius,
                                      segment.Radius);
            SweepDirection finishSweepDirection =
                segment.TurnDirection == Constants.TurnDirection.Clockwise
                    ? SweepDirection.Clockwise
                    : SweepDirection.Counterclockwise;

            bool isLargeArc = CalculateIsLargeArc(segment);

            ArcSegment arcSegment = CreateArcSegment(finishEndPoint,
                                                     finishSize,
                                                     finishSweepDirection,
                                                     isLargeArc);

            return arcSegment;
        }

        public ArcSegment CreateArcSegment(Point point,
                                           Size size,
                                           SweepDirection sweepDirection,
                                           bool isLargeArc = false)
        {
            const double rotationAngle = 0.0;
            const bool isStroked = true;

            var arcSegment = new ArcSegment(point,
                                            size,
                                            rotationAngle,
                                            isLargeArc,
                                            sweepDirection,
                                            isStroked);

            return arcSegment;
        }

        public Point PointRelativeToOrigin(Geometry.Shapes.Point point)
        {
            m_Converter.GeometryPoint = point;
            m_Converter.Convert();

            return m_Converter.Point;
        }

        internal bool CalculateIsLargeArc(ITurnCircleArcSegment segment)
        {
            Geometry.Shapes.Point startPoint = segment.CentrePoint;
            Geometry.Shapes.Point endPoint = segment.CircleOrigin == Constants.CircleOrigin.Start
                                                 ? segment.EndPoint
                                                 : segment.StartPoint;
            Geometry.Shapes.Point checkPoint = segment.CircleOrigin == Constants.CircleOrigin.Start
                                                   ? segment.StartPoint
                                                   : segment.EndPoint;

            var line = new Line(-1,
                                startPoint,
                                endPoint);

            Constants.TurnDirection turnDirection = line.TurnDirectionToPoint(checkPoint);

            bool isLargeArc;

            if ( segment.CircleOrigin == Constants.CircleOrigin.Start )
            {
                isLargeArc = turnDirection == segment.TurnDirection;
            }
            else
            {
                isLargeArc = turnDirection != segment.TurnDirection;
            }

            return isLargeArc;
        }
    }
}