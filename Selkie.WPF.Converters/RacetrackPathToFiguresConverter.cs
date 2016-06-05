using System.Linq;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class RacetrackPathToFiguresConverter : IRacetrackPathToFiguresConverter
    {
        public RacetrackPathToFiguresConverter([NotNull] IRacetrackPathTurnToFiguresConverter racetrackPathTurn,
                                               [NotNull] IRacetrackPathUTurnToFiguresConverter racetrackPathUTurn)
        {
            m_RacetrackPathTurn = racetrackPathTurn;
            m_RacetrackPathUTurn = racetrackPathUTurn;
        }

        public ISelkieLogger Logger { get; set; }
        private readonly IRacetrackPathTurnToFiguresConverter m_RacetrackPathTurn;
        private readonly IRacetrackPathUTurnToFiguresConverter m_RacetrackPathUTurn;
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

            if ( IsNormalTurn(segments [ 1 ]) )
            {
                m_FiguresCollection = ConvertTurnPath(m_Path);
            }
            else if ( IsUTurn(segments [ 1 ]) )
            {
                m_FiguresCollection = ConvertUTurnPath(m_Path);
            }
            else
            {
                string message = "Could not determine racetrack type! - Racetrack path: {0}".Inject(m_Path);

                Logger.Error(message);
            }
        }

        internal PathFigureCollection ConvertTurnPath([NotNull] IPath path)
        {
            m_RacetrackPathTurn.Path = path;
            m_RacetrackPathTurn.Convert();

            return m_RacetrackPathTurn.FiguresCollection;
        }

        internal PathFigureCollection ConvertUTurnPath([NotNull] IPath path)
        {
            m_RacetrackPathUTurn.Path = path;
            m_RacetrackPathUTurn.Convert();

            return m_RacetrackPathUTurn.FiguresCollection;
        }

        internal bool IsNormalTurn([NotNull] IPolylineSegment segment)
        {
            return segment is ILine;
        }

        internal bool IsUTurn([NotNull] IPolylineSegment segment)
        {
            return segment is ITurnCircleArcSegment;
        }
    }
}