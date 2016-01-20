using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Services.Common.Dto;

namespace Selkie.Framework.Converters
{
    public interface IPathDtoToPath : IConverter
    {
        [NotNull]
        PathDto Dto { get; set; }

        [NotNull]
        IPath Path { get; set; }
    }
}