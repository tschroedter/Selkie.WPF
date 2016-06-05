using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common.Converters
{
    public class LinesToTransferPointsConverter : ILinesToTransferPointsConverter
    {
        public LinesToTransferPointsConverter([NotNull] ILine from,
                                              Constants.LineDirection fromDirection,
                                              [NotNull] ILine to,
                                              Constants.LineDirection toDirection)
        {
            m_From = from;
            m_FromDirection = fromDirection;
            m_To = to;
            m_ToDirection = toDirection;
        }

        public ILine From
        {
            get
            {
                return m_From;
            }
        }

        public Constants.LineDirection FromDirection
        {
            get
            {
                return m_FromDirection;
            }
        }

        public ILine To
        {
            get
            {
                return m_To;
            }
        }

        public Constants.LineDirection ToDirection
        {
            get
            {
                return m_ToDirection;
            }
        }

        public IEnumerable <Point> Points
        {
            get
            {
                return m_Points;
            }
        }

        private readonly ILine m_From;
        private readonly Constants.LineDirection m_FromDirection;
        private readonly ILine m_To;
        private readonly Constants.LineDirection m_ToDirection;
        private IEnumerable <Point> m_Points = new List <Point>();

        public void Convert()
        {
            m_Points = CreatePointsForTransfer(m_From,
                                               m_FromDirection,
                                               m_To,
                                               m_ToDirection);
        }

        internal IEnumerable <Point> CreatePointsForTransfer(ILine from,
                                                             Constants.LineDirection fromDirection,
                                                             ILine to,
                                                             Constants.LineDirection toDirection)
        {
            var points = new List <Point>();

            if ( fromDirection == Constants.LineDirection.Forward
                 &&
                 toDirection == Constants.LineDirection.Forward )
            {
                points.Add(from.EndPoint);
                points.Add(to.StartPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Forward
                      &&
                      toDirection == Constants.LineDirection.Reverse )
            {
                points.Add(from.EndPoint);
                points.Add(to.EndPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Reverse
                      &&
                      toDirection == Constants.LineDirection.Forward )
            {
                points.Add(from.StartPoint);
                points.Add(to.StartPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Reverse
                      &&
                      toDirection == Constants.LineDirection.Reverse )
            {
                points.Add(from.StartPoint);
                points.Add(to.EndPoint);
            }

            return points;
        }
    }
}