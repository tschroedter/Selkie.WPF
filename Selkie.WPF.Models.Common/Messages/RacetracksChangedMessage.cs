using System.Diagnostics.CodeAnalysis;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Models.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class RacetracksResponseMessage
    {
        public IRacetracks Racetracks { get; set; }
    }
}