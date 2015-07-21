using JetBrains.Annotations;

namespace Selkie.WPF.Models.Interfaces
{
    public interface IPheromonesModel : IModel
    {
        [NotNull]
        double[][] Values { get; }

        double Minimum { get; }
        double Maximum { get; }
        double Average { get; }
    }
}