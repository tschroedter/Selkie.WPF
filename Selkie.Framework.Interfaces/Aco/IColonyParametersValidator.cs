using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces.Aco
{
    public interface IColonyParametersValidator
    {
        void Validate([NotNull] IColonyParameters colonyParameters);
    }
}