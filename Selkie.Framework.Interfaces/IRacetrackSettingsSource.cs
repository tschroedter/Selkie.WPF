namespace Selkie.Framework.Interfaces
{
    public interface IRacetrackSettingsSource
    {
        double TurnRadiusForPort { get; }
        bool IsPortTurnAllowed { get; }
        bool IsStarboardTurnAllowed { get; }
        double TurnRadiusForStarboard { get; }
    }
}