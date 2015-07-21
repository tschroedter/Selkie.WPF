namespace Selkie.WPF.Models.Common.Messages
{
    public class RacetrackSettingsSetMessage
    {
        public double TurnRadius { get; set; }
        public bool IsPortTurnAllowed { get; set; }
        public bool IsStarboardTurnAllowed { get; set; }
    }
}