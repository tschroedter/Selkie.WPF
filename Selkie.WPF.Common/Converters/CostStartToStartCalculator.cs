using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters
{
    public class CostStartToStartCalculator : BaseCostCalculator, ICostStartToStartCalculator
    {
        internal override double CalculateRacetrackCost(int fromLineId,
                                                        int toLineId)
        {
            return Racetracks.ReverseToForward[fromLineId][toLineId].Distance.Length;
        }
    }

    public interface ICostStartToStartCalculator : IBaseCostCalculator
    {
    }
}