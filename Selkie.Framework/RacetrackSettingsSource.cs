using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class RacetrackSettingsSource : IRacetrackSettingsSource
    {
        public RacetrackSettingsSource(double turnRadiusForPort,
                                       double turnRadiusForStarboard,
                                       bool isPortTurnAllowed,
                                       bool isStarboardTurnAllowed)
        {
            TurnRadiusForPort = turnRadiusForPort;
            TurnRadiusForStarboard = turnRadiusForStarboard;
            IsPortTurnAllowed = isPortTurnAllowed;
            IsStarboardTurnAllowed = isStarboardTurnAllowed;
        }

        public double TurnRadiusForPort { get; private set; }

        public double TurnRadiusForStarboard { get; private set; }

        public bool IsPortTurnAllowed { get; private set; }

        public bool IsStarboardTurnAllowed { get; private set; }
    }
}