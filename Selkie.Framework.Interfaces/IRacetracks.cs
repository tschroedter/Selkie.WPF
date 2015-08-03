using JetBrains.Annotations;

namespace Selkie.Framework.Interfaces
{
    public interface IRacetracks
    {
        [NotNull]
        IPath[][] ForwardToForward { get; }

        [NotNull]
        IPath[][] ForwardToReverse { get; }

        [NotNull]
        IPath[][] ReverseToForward { get; }

        [NotNull]
        IPath[][] ReverseToReverse { get; }

        bool IsUnknown { get; }
    }
}