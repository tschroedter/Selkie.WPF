using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyPheromonesMessage
    {
        private double[][] m_Values = new double[0][];

        [NotNull]
        public double[][] Values
        {
            get
            {
                return m_Values;
            }
            set
            {
                m_Values = value;
            }
        }

        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Average { get; set; }
    }
}