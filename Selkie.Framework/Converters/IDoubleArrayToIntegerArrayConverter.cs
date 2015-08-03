using JetBrains.Annotations;
using Selkie.Framework.Interfaces.Converters;

namespace Selkie.Framework.Converters
{
    public interface IDoubleArrayToIntegerArrayConverter : IConverter
    {
        [NotNull]
        int[][] IntegerMatrix { get; }

        [NotNull]
        double[][] DoubleMatrix { get; set; }
    }
}