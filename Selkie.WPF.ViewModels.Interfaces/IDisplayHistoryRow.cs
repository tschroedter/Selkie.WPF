using System.Collections.Generic;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IDisplayHistoryRow
    {
        double LengthDelta { get; }
        double LengthDeltaInPercent { get; }
        int Interation { get; }
        double Length { get; }
        IEnumerable <int> Trail { get; }
        double Alpha { get; }
        double Beta { get; }
        double Gamma { get; }
        string Type { get; }
        string TrailRaw { get; }
    }
}