using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces
{
    public interface IColonyParameters
    {
        [NotNull]
        int[][] CostMatrix { get; set; }

        [NotNull]
        int[] CostPerFeature { get; set; }

        bool IsFixedStartNode { get; set; }
        int FixedStartNode { get; set; }
    }
}