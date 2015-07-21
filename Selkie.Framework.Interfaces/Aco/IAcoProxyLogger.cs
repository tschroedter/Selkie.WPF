using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces.Aco
{
    public interface IAcoProxyLogger
    {
        void LogCostPerLine([NotNull] int[] costPerLine);
        void LogCostMatrix([NotNull] int[][] matrix);
        void Error([NotNull] string costMatrixIsNotSet);
        void Info([NotNull] string toString);
    }
}