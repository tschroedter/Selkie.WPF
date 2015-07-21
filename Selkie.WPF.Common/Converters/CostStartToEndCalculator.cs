using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters
{
    public class CostStartToEndCalculator : BaseCostCalculator, ICostStartToEndCalculator
    {
        internal override double CalculateRacetrackCost(int fromLineId,
                                                        int toLineId)
        {
            return Racetracks.ReverseToReverse[fromLineId][toLineId].Distance.Length;
        }
    }

    public interface ICostStartToEndCalculator : IBaseCostCalculator
    {
    }
}