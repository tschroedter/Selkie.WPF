using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters
{
    public class CostEndToStartCalculator : BaseCostCalculator, ICostEndToStartCalculator
    {
        internal override double CalculateRacetrackCost(int fromLineId,
                                                        int toLineId)
        {
            return Racetracks.ForwardToForward[fromLineId][toLineId].Distance.Length;
        }
    }

    public interface ICostEndToStartCalculator : IBaseCostCalculator
    {
    }
}