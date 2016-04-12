using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.Framework.Interfaces
{
    public interface IAntSettingsSourceFactory : ITypedFactory
    {
        IAntSettingsSource Create(bool isFixedStartNode,
                                  int fixedStartNode);

        void Release([NotNull] IAntSettingsSource antSettingsSourceSource);
    }
}