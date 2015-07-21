using System.Collections.Generic;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface ITrailHistoryViewModel : IViewModel
    {
        IEnumerable <IDisplayHistoryRow> Rows { get; }
    }
}