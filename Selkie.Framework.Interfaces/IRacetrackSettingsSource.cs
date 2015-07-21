namespace Selkie.Framework.Interfaces
{
    public interface IRacetrackSettingsSource
    {
        double TurnRadius { get; }
        bool IsPortTurnAllowed { get; }
        bool IsStarboardTurnAllowed { get; }
    }
}