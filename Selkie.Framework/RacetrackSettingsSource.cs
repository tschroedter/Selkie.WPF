using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class RacetrackSettingsSource : IRacetrackSettingsSource
    {
        private readonly bool m_IsPortTurnAllowed;
        private readonly bool m_IsStarboardTurnAllowed;
        private readonly double m_TurnRadius;

        public RacetrackSettingsSource(double turnRadius,
                                       bool isPortTurnAllowed,
                                       bool isStarboardTurnAllowed)
        {
            m_TurnRadius = turnRadius;
            m_IsPortTurnAllowed = isPortTurnAllowed;
            m_IsStarboardTurnAllowed = isStarboardTurnAllowed;
        }

        public double TurnRadius
        {
            get
            {
                return m_TurnRadius;
            }
        }

        public bool IsPortTurnAllowed
        {
            get
            {
                return m_IsPortTurnAllowed;
            }
        }

        public bool IsStarboardTurnAllowed
        {
            get
            {
                return m_IsStarboardTurnAllowed;
            }
        }
    }
}