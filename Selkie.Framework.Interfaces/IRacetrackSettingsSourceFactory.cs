using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.Framework.Interfaces
{
    public interface IRacetrackSettingsSourceFactory : ITypedFactory
    {
        IRacetrackSettingsSource Create(double turnRadiusForPort,
                                        double turnRadiusForStarboard,
                                        bool isPortTurnAllowed,
                                        bool isStarboardTurnAllowed);

        void Release([NotNull] IRacetrackSettingsSource linesSource);
    }
}