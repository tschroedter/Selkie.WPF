using System.Collections.Generic;
using System.Linq;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class GrayscaleConverter : IGrayscaleConverter
    {
        private readonly IDoubleToIntegerConverter m_DoubleToIntegerConverter;
        private double m_Maximum = 128.0;
        private double m_Minimum = -128.0;
        private int m_NumberOfPossibleValues = 255;

        private double[][] m_Pheromones =
        {
            new double[]
            {
            }
        };

        public GrayscaleConverter(IDoubleToIntegerConverter doubleToIntegerConverter)
        {
            m_DoubleToIntegerConverter = doubleToIntegerConverter;
        }

        internal IEnumerable <int[]> ToInteger(double[][] pheromones,
                                               int numberOfPossibleValues)
        {
            m_DoubleToIntegerConverter.NumberOfPossibleValues = m_NumberOfPossibleValues;
            m_DoubleToIntegerConverter.Minimum = m_Minimum;
            m_DoubleToIntegerConverter.Interval = ( m_Maximum - m_Minimum ) / m_NumberOfPossibleValues;

            var toInteger = new int[pheromones.Length][];

            for ( var i = 0 ; i < pheromones.Length ; i++ )
            {
                double[] current = pheromones [ i ];
                var currentArray = new int[current.Length];

                for ( var j = 0 ; j < current.Length ; j++ )
                {
                    m_DoubleToIntegerConverter.Value = current [ j ];
                    m_DoubleToIntegerConverter.Convert();

                    currentArray [ j ] = m_DoubleToIntegerConverter.Integer;
                }

                toInteger [ i ] = currentArray;
            }

            return toInteger;
        }

        private List <List <int>> CreatePheromones(IEnumerable <int[]> pheromones)
        {
            return pheromones.Select(t => t.ToList()).ToList();
        }

        #region IGrayscaleConverter Members

        public double Minimum
        {
            get
            {
                return m_Minimum;
            }
            set
            {
                m_Minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return m_Maximum;
            }
            set
            {
                m_Maximum = value;
            }
        }

        public int NumberOfPossibleValues
        {
            get
            {
                return m_NumberOfPossibleValues;
            }
            set
            {
                m_NumberOfPossibleValues = value;
            }
        }

        public double[][] Pheromones
        {
            get
            {
                return m_Pheromones;
            }
            set
            {
                m_Pheromones = value;
            }
        }

        public List <List <int>> Grayscale { get; private set; }

        public void Convert()
        {
            IEnumerable <int[]> tointeger = ToInteger(m_Pheromones,
                                                      255);

            Grayscale = CreatePheromones(tointeger);
        }

        #endregion
    }
}