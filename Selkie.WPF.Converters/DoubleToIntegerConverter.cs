using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class DoubleToIntegerConverter : IDoubleToIntegerConverter
    {
        public int NumberOfPossibleValues { get; set; }
        public double Minimum { get; set; }
        public double Interval { get; set; }
        public double Value { get; set; }
        public int Integer { get; private set; }

        public void Convert()
        {
            Integer = DoubleToInteger(NumberOfPossibleValues,
                                      Minimum,
                                      Interval,
                                      Value);
        }

        internal int DoubleToInteger(int numberOfPossibleValues,
                                     double minimum,
                                     double interval,
                                     double value)
        {
            if ( interval <= 0.0 )
            {
                return 0;
            }

            double currentInterval = minimum + interval;

            int grayValue;

            for ( grayValue = 0 ; grayValue < numberOfPossibleValues + 1 ; grayValue++, currentInterval += interval )
            {
                if ( value <= currentInterval )
                {
                    return grayValue;
                }
            }

            return grayValue - 2;
        }
    }
}