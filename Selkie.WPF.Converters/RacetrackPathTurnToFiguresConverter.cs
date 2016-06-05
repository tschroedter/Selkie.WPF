using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Converters.Interfaces;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters
{
    public class RacetrackPathTurnToFiguresConverter : IRacetrackPathTurnToFiguresConverter
    {
        public RacetrackPathTurnToFiguresConverter(IPathSegmentHelper helper)
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
            m_FiguresCollection = CreateFigures(m_Path);
        }

        internal PathSegment ConvertSegment([NotNull] IPolylineSegment segment)
        {
            var arcSegment = segment as ITurnCircleArcSegment;

            if ( arcSegment != null )
            {
                return m_Helper.SegmentToArcSegment(arcSegment);
            }

            var lineSegment = segment as ILine;

            if ( lineSegment != null )
            {
                return m_Helper.SegmentToLineSegment(lineSegment);
            }

            throw new ArgumentException("Unknown PolylineSegment '{0}'!".Inject(segment));
        }

        internal PathFigureCollection CreateFigures([NotNull] IPath path)
        {
            List <PathSegment> pathSegments = path.Segments.Select(ConvertSegment).ToList();

            Point startPoint = m_Helper.PointRelativeToOrigin(path.StartPoint);

            var pathFigure = new PathFigure(startPoint,
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