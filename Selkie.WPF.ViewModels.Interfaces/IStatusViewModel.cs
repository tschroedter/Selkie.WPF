using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IStatusViewModel : IViewModel
    {
        [NotNull]
        string Status { get; }
    }
}