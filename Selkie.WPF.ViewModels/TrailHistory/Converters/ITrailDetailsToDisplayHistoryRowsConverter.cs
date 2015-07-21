using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using IConverter = Selkie.WPF.ViewModels.Interfaces.IConverter;

namespace Selkie.WPF.ViewModels.TrailHistory.Converters
{
    public interface ITrailDetailsToDisplayHistoryRowsConverter : IConverter
    {
        [NotNull]
        IEnumerable <IDisplayHistoryRow> DisplayHistoryRows { get; }

        [NotNull]
        IEnumerable <ITrailDetails> Trails { get; set; }
    }
}