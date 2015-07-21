using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Models.Common.Messages
{
    public class RacetracksChangedMessage
    {
        public IRacetracks Racetracks { get; set; }
    }
}