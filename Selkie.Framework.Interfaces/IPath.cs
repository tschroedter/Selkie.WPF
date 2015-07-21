using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Interfaces
{
    public interface IPath
    {
        Point StartPoint { get; }
        Point EndPoint { get; }
        IPolyline Polyline { get; }
        IEnumerable <IPolylineSegment> Segments { get; }
        bool IsUnknown { get; }
        Distance Distance { get; }
        void Add([NotNull] IPolylineSegment polylineSegment);
        IPath Reverse();
    }
}