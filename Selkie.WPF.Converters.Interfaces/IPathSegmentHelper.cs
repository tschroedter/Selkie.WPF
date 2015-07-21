using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using ArcSegment = System.Windows.Media.ArcSegment;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IPathSegmentHelper
    {
        LineSegment SegmentToLineSegment([NotNull] ILine segment);
        ArcSegment SegmentToArcSegment([NotNull] ITurnCircleArcSegment segment);

        ArcSegment CreateArcSegment(Point point,
                                    Size size,
                                    SweepDirection sweepDirection,
                                    bool isLargeArc = false);

        Point PointRelativeToOrigin(Geometry.Shapes.Point point);
    }
}