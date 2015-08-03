using Selkie.Common;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common.Converters
{
    public class CostForLineSwitchCalculator : ICostForLineSwitchCalculator
    {
        public CostForLineSwitchCalculator(ILine from,
                                           Constants.LineDirection fromDirection,
                                           ILine to,
                                           Constants.LineDirection toDirection)
        {
            From = from;
            FromDirection = fromDirection;
            To = to;
            ToDirection = toDirection;
        }

        public ILine From { get; set; }
        public Constants.LineDirection FromDirection { get; set; }
        public ILine To { get; set; }
        public Constants.LineDirection ToDirection { get; set; }
        public double Cost { get; private set; }

        public void Calculate()
        {
            Cost = CostForLineSwitch(From,
                                     FromDirection,
                                     To,
                                     ToDirection);
        }

        internal double CostForLineSwitch(ILine from,
                                          Constants.LineDirection fromDirection,
                                          ILine to,
                                          Constants.LineDirection toDirection)
        {
            double cost = double.MaxValue;

            if ( fromDirection == Constants.LineDirection.Forward &&
                 toDirection == Constants.LineDirection.Forward )
            {
                cost = from.EndPoint.DistanceTo(to.StartPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Forward &&
                      toDirection == Constants.LineDirection.Reverse )
            {
                cost = from.EndPoint.DistanceTo(to.EndPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Reverse &&
                      toDirection == Constants.LineDirection.Forward )
            {
                cost = from.StartPoint.DistanceTo(to.StartPoint);
            }
            else if ( fromDirection == Constants.LineDirection.Reverse &&
                      toDirection == Constants.LineDirection.Reverse )
            {
                cost = from.StartPoint.DistanceTo(to.EndPoint);
            }

            return cost;
        }
    }
}