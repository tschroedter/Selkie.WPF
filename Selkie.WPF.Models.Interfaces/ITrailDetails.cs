using System.Collections.Generic;

namespace Selkie.WPF.Models.Interfaces
{
    public interface ITrailDetails
    {
        double Alpha { get; }
        double Beta { get; }
        double Gamma { get; }
        double Length { get; }
        IEnumerable <int> Trail { get; }
        string Type { get; }
        bool IsUnknown { get; }
        double LengthDeltaInPercent { get; }
        double LengthDelta { get; }
        int Interation { get; }
    }
}