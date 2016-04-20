using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IPheromonesViewModel : IViewModel
    {
        string Minimum { get; }
        string Maximum { get; }
        string Average { get; }
        bool IsShowPheromones { get; set; }
    }
}