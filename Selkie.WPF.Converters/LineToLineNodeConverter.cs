using System;
using System.Collections.Generic;
using Selkie.Common;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Converters;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class LineToLineNodeConverter
        : ILineToLineNodeConverter,
          IEquatable <LineToLineNodeConverter>
    {
        private ILine m_From = Line.Unknown;
        private Constants.LineDirection m_FromDirection = Constants.LineDirection.Forward;
        private IEnumerable <Point> m_Points = new List <Point>();
        private ILine m_To = Line.Unknown;
        private Constants.LineDirection m_ToDirection = Constants.LineDirection.Forward;

        public override string ToString()
        {
            string text = From.Id + " (" + FromDirection + ")" +
                          " - " +
                          To.Id + " (" + ToDirection + ")";

            return text;
        }

        internal double CalculateCost(ILine from,
                                      Constants.LineDirection fromDirection,
                                      ILine to,
                                      Constants.LineDirection toDirection)
        {
            var calculator = new CostForLineSwitchCalculator(from,
                                                             fromDirection,
                                                             to,
                                                             toDirection);

            calculator.Calculate();

            double length = From.Length + To.Length + calculator.Cost;

            return length;
        }

        internal double CalculateCostStartToStart(ILine from,
                                                  Constants.LineDirection fromDirection,
                                                  ILine to,
                                                  Constants.LineDirection toDirection)
        {
            var calculator = new CostForLineSwitchCalculator(from,
                                                             fromDirection,
                                                             to,
                                                             toDirection);

            calculator.Calculate();

            double length = From.Length + calculator.Cost;

            return length;
        }

        internal void Convert(ILine from,
                              Constants.LineDirection fromDirection,
                              ILine to,
                              Constants.LineDirection toDirection)
        {
            Cost = CalculateCost(from,
                                 fromDirection,
                                 to,
                                 toDirection);

            var converter = new LinesToTransferPointsConverter(from,
                                                               fromDirection,
                                                               to,
                                                               toDirection);

            converter.Convert();

            m_Points = converter.Points;
        }

        #region ILineToLineNodeConverter Members

        public void Convert()
        {
            Convert(m_From,
                    m_FromDirection,
                    m_To,
                    m_ToDirection);
        }

        public ILine From
        {
            get
            {
                return m_From;
            }
            set
            {
                m_From = value;
            }
        }

        public Constants.LineDirection FromDirection
        {
            get
            {
                return m_FromDirection;
            }
            set
            {
                m_FromDirection = value;
            }
        }

        public ILine To
        {
            get
            {
                return m_To;
            }
            set
            {
                m_To = value;
            }
        }

        public Constants.LineDirection ToDirection
        {
            get
            {
                return m_ToDirection;
            }
            set
            {
                m_ToDirection = value;
            }
        }

        public double Cost { get; private set; }

        public IEnumerable <Point> Points
        {
            get
            {
                return m_Points;
            }
        }

        #endregion

        #region IEquatable

        public bool Equals(LineToLineNodeConverter other)
        {
            if ( ReferenceEquals(null,
                                 other) )
            {
                return false;
            }
            if ( ReferenceEquals(this,
                                 other) )
            {
                return true;
            }
            return Equals(other.From,
                          From) && Equals(other.FromDirection,
                                          FromDirection) && Equals(other.To,
                                                                   To) &&
                   Equals(other.ToDirection,
                          ToDirection) && other.Cost.Equals(Cost);
        }

        public override bool Equals(object obj)
        {
            if ( ReferenceEquals(null,
                                 obj) )
            {
                return false;
            }
            if ( ReferenceEquals(this,
                                 obj) )
            {
                return true;
            }
            return obj.GetType() == typeof( LineToLineNodeConverter ) && Equals(( LineToLineNodeConverter ) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = From.GetHashCode();
                result = ( result * 397 ) ^ FromDirection.GetHashCode();
                result = ( result * 397 ) ^ To.GetHashCode();
                result = ( result * 397 ) ^ ToDirection.GetHashCode();
                result = ( result * 397 ) ^ Cost.GetHashCode();
                return result;
            }
        }

        #endregion
    }
}