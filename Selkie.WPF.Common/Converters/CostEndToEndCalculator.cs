using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters
{
    public class CostEndToEndCalculator : BaseCostCalculator, ICostEndToEndCalculator
    {
        internal override double CalculateRacetrackCost(int fromLineId,
                                                        int toLineId)
        {
            return Racetracks.ForwardToReverse[fromLineId][toLineId].Distance.Length;
        }
    }

    public interface ICostEndToEndCalculator : IBaseCostCalculator
    {
    }
}