using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;
using ArcSegment = System.Windows.Media.ArcSegment;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters
{
    public class RacetrackPathUTurnToFiguresConverter : IRacetrackPathUTurnToFiguresConverter
    {
        public RacetrackPathUTurnToFiguresConverter([NotNull] IPathSegmentHelper helper)
        {
            m_Helper = helper;
        }

        private readonly IPathSegmentHelper m_Helper;
        private PathFigureCollection m_FiguresCollection = new PathFigureCollection();
        private IPath m_Path = Framework.Common.Path.Unknown;

        public IPath Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        public PathFigureCollection FiguresCollection
        {
            get
            {
                return m_FiguresCollection;
            }
        }

        public void Convert()
        {
            if ( m_Path.IsUnknown )
            {
                return;
            }

            IPolylineSegment[] segments = m_Path.Segments.ToArray();

            if ( segments.Length != 3 )
            {
                return;
            }

            var startSegment = segments [ 0 ] as ITurnCircleArcSegment;
            var middleSegment = segments [ 1 ] as ITurnCircleArcSegment;
            var endSegment = segments [ 2 ] as ITurnCircleArcSegment;

            if ( startSegment == null ||
                 middleSegment == null ||
                 endSegment == null )
            {
                return;
            }

            m_FiguresCollection = CreateFigures(startSegment,
                                                middleSegment,
                                                endSegment);
        }

        internal PathFigureCollection CreateFigures(ITurnCircleArcSegment startSegment,
                                                    ITurnCircleArcSegment middleSegment,
                                                    ITurnCircleArcSegment endSegment)

        {
            ArcSegment arcStartSegment = m_Helper.SegmentToArcSegment(startSegment);
            ArcSegment arcMiddleSegment = m_Helper.SegmentToArcSegment(middleSegment);
            ArcSegment arcFinishSegment = m_Helper.SegmentToArcSegment(endSegment);

            var pathSegments = new List <PathSegment>
                               {
                                   arcStartSegment,
                                   arcMiddleSegment,
                                   arcFinishSegment
                               };

            Point start = m_Helper.PointRelativeToOrigin(startSegment.StartPoint);

            var pathFigure = new PathFigure(start,
                                            pathSegments,
                                            false);

            var figures = new PathFigureCollection
                          {
                              pathFigure
                          };

            return figures;
        }
    }
}