using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces.Aco
{
    public interface IAcoProxyLogger
    {
        void Error([NotNull] string costMatrixIsNotSet);
        void Info([NotNull] string toString);
        void LogCostMatrix([NotNull] int[][] matrix);
        void LogCostPerFeature([NotNull] int[] costPerFeature);
    }
}