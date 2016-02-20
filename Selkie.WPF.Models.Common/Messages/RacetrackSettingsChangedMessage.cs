namespace Selkie.WPF.Models.Common.Messages
{
    public class RacetrackSettingsChangedMessage
    {
        public double TurnRadiusForPort { get; set; }
        public double TurnRadiusForStarboard { get; set; }
        public bool IsPortTurnAllowed { get; set; }
        public bool IsStarboardTurnAllowed { get; set; }
    }
}