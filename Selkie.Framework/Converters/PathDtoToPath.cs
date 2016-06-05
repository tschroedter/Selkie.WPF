using System;
using JetBrains.Annotations;
using Selkie.Framework.Common;
using Selkie.Framework.Interfaces;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework.Converters
{
    [ProjectComponent(Lifestyle.Transient)]
    public class PathDtoToPath : IPathDtoToPath
    {
        private PathDto m_Dto = new PathDto();
        private IPath m_Path = Common.Path.Unknown;

        public void Convert()
        {
            IPolyline polyline = CreatePolyline(m_Dto.Polyline);
            m_Path = new Path(polyline);
        }

        public PathDto Dto
        {
            get
            {
                return m_Dto;
            }
            set
            {
                m_Dto = value;
            }
        }

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

        internal ICircle CreateCircleFormCircleDto(CircleDto dto)
        {
            if ( dto.IsUnknown )
            {
                return Circle.Unknown;
            }

            Point centrePoint = CreatePointFromPointDto(dto.CentrePoint);
            double radius = dto.Radius;
            var circle = new Circle(centrePoint,
                                    radius);

            return circle;
        }

        internal Constants.LineDirection CreateLineDirectionFromString(string runDirectionString)
        {
            Constants.LineDirection lineDirection;
            if ( Enum.TryParse(runDirectionString,
                               out lineDirection) )
            {
                return lineDirection;
            }

            throw new ArgumentException("Can't parse string {0} to determine LineDirection!".Inject(runDirectionString));
        }

        internal Point CreatePointFromPointDto(PointDto dto)
        {
            return new Point(dto.X,
                             dto.Y);
        }

        internal IPolyline CreatePolyline([NotNull] PolylineDto polylineDto)
        {
            var polyline = new Polyline(0,
                                        Constants.LineDirection.Forward);
            var count = 0;

            foreach ( SegmentDto segment in polylineDto.Segments )
            {
                Constants.CircleOrigin origin = count++ <= 1
                                                    ? Constants.CircleOrigin.Start
                                                    : Constants.CircleOrigin.Finish;

                IPolylineSegment polylineSegment = CreatePolylineSegmentFromDto(segment,
                                                                                origin);

                polyline.AddSegment(polylineSegment);
            }

            return polyline;
        }

        internal IPolylineSegment CreatePolylineSegmentFromDto(SegmentDto segment,
                                                               Constants.CircleOrigin origin)
        {
            IPolylineSegment polylineSegment;

            var dto = segment as ArcSegmentDto;
            if ( dto != null )
            {
                polylineSegment = CreateTurnCircleArcSegment(dto,
                                                             origin);
            }
            else
            {
                var segmentDto = segment as LineSegmentDto;
                if ( segmentDto != null )
                {
                    polylineSegment = CreateLineFromLineDto(segmentDto);
                }
                else
                {
                    throw new ArgumentException(
                        "Unknown polyline segment {0}!".Inject(segment.GetType().FullName));
                }
            }

            return polylineSegment;
        }

        internal Constants.TurnDirection CreateTurnDirectionFromString(string turnDirectionString)
        {
            Constants.TurnDirection turnDirection;
            if ( Enum.TryParse(turnDirectionString,
                               out turnDirection) )
            {
                return turnDirection;
            }

            throw new ArgumentException("Can't parse string {0} to determine TurnDirection!".Inject(turnDirectionString));
        }

        private ILine CreateLineFromLineDto(LineSegmentDto dto)
        {
            Point startPoint = CreatePointFromPointDto(dto.StartPoint);
            Point endPoint = CreatePointFromPointDto(dto.EndPoint);
            Constants.LineDirection lineDirection = CreateLineDirectionFromString(dto.RunDirection);
            var line = new Line(startPoint,
                                endPoint,
                                lineDirection);

            return line;
        }

        private ITurnCircleArcSegment CreateTurnCircleArcSegment(ArcSegmentDto segment,
                                                                 Constants.CircleOrigin origin)
        {
            ICircle circle = CreateCircleFormCircleDto(segment.Circle);
            Point startPoint = CreatePointFromPointDto(segment.StartPoint);
            Point endPoint = CreatePointFromPointDto(segment.EndPoint);
            Constants.TurnDirection turnDirection = CreateTurnDirectionFromString(segment.TurnDirection);

            ITurnCircleArcSegment turnCircleArcSegment = new TurnCircleArcSegment(circle,
                                                                                  turnDirection,
                                                                                  origin,
                                                                                  startPoint,
                                                                                  endPoint);
            // todo equals for TurnCircleArcSegment

            return turnCircleArcSegment;
        }
    }
}