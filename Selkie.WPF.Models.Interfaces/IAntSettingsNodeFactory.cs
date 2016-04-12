using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.WPF.Models.Interfaces
{
    public interface IAntSettingsNodeFactory : ITypedFactory
    {
        IAntSettingsNode Create(int id,
                                [NotNull] string description);

        void Release([NotNull] IAntSettingsNode node);
    }
}