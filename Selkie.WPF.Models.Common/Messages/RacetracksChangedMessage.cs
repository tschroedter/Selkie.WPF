using System.Diagnostics.CodeAnalysis;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Models.Common.Messages
{
    //ncrunch: no coverage start
    [ExcludeFromCodeCoverage]
    public class RacetracksResponseMessage
    {
        public IRacetracks Racetracks { get; set; }
    }
}