using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyLinesResponseMessage
    {
        public ColonyLinesResponseMessage()
        {
            Lines = new ILine[0];
        }

        [NotNull]
        public IEnumerable <ILine> Lines { get; set; } // todo maybe use Dtos
    }
}