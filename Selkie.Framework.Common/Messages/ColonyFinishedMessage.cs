using System;
using System.Diagnostics.CodeAnalysis;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyFinishedMessage
    {
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public int Times { get; set; }
    }
}