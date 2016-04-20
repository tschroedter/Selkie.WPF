using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyAvailableTestLinesResponseMessage
    {
        public ColonyAvailableTestLinesResponseMessage()
        {
            Types = new string[0];
        }

        [NotNull]
        public IEnumerable <string> Types { get; set; }
    }
}