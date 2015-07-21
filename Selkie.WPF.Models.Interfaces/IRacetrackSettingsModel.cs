namespace Selkie.WPF.Models.Interfaces
{
    public interface IRacetrackSettingsModel : IModel
    {
        double TurnRadius { get; }
        bool IsPortTurnAllowed { get; }
        bool IsStarboardTurnAllowed { get; }
    }
}