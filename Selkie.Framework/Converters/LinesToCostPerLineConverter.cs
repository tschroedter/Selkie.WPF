using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Converters
{
    public sealed class LinesToCostPerLineConverter : ILinesToCostPerLineConverter
    {
        private int[] m_CostPerLine;
        private IEnumerable <ILine> m_Lines = new ILine[0];

        internal int[] CreateCostPerLine([NotNull] IEnumerable <ILine> lines)
        {
            ILine[] array = lines.ToArray();

            var costs = new int[array.Length * 2];

            var index = 0;

            foreach ( ILine line in array )
            {
                costs [ index++ ] = ( int ) line.Length;
                costs [ index++ ] = ( int ) line.Length;
            }

            return costs;
        }

        #region ILinesToMatrixConverter Members

        public void Convert()
        {
            m_CostPerLine = CreateCostPerLine(Lines);
        }

        public IEnumerable <ILine> Lines
        {
            get
            {
                return m_Lines;
            }
            set
            {
                m_Lines = value;
            }
        }

        public IEnumerable <int> CostPerLine
        {
            get
            {
                return m_CostPerLine;
            }
        }

        #endregion
    }
}