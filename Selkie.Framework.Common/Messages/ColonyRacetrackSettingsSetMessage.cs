using System.Diagnostics.CodeAnalysis;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyRacetrackSettingsSetMessage
    {
        public double TurnRadiusForPort { get; set; }
        public double TurnRadiusForStarboard { get; set; }
        public bool IsPortTurnAllowed { get; set; }
        public bool IsStarboardTurnAllowed { get; set; }
    }
}