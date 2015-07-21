using System.Collections.Generic;

namespace Selkie.WPF.Models.Interfaces
{
    public interface ITrailHistoryModel : IModel
    {
        IEnumerable <ITrailDetails> Trails { get; }
        ITrailDetails FirsTrailDetails { get; }
    }
}