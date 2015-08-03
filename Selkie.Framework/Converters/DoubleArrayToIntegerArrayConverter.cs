namespace Selkie.Framework.Converters
{
    public class DoubleArrayToIntegerArrayConverter : IDoubleArrayToIntegerArrayConverter
    {
        private double[][] m_DoubleMatrix = new double[0][];
        private int[][] m_IntegerMatrix = new int[0][];

        public void Convert()
        {
            m_IntegerMatrix = ConvertToInteger(m_DoubleMatrix);
        }

        public int[][] IntegerMatrix
        {
            get
            {
                return m_IntegerMatrix;
            }
        }

        public double[][] DoubleMatrix
        {
            get
            {
                return m_DoubleMatrix;
            }
            set
            {
                m_DoubleMatrix = value;
            }
        }

        private int[][] ConvertToInteger(double[][] doubleMatrix)
        {
            int size = doubleMatrix.GetLength(0);

            var intMatrix = new int[size][];

            for ( var i = 0 ; i < size ; i++ )
            {
                var values = new int[size];
                intMatrix [ i ] = values;

                for ( var j = 0 ; j < size ; j++ )
                {
                    values [ j ] = ( int ) doubleMatrix [ i ] [ j ];
                }
            }

            return intMatrix;
        }
    }
}