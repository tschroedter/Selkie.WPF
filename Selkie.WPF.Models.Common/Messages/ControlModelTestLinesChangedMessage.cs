using System.Collections.Generic;

namespace Selkie.WPF.Models.Common.Messages
{
    public class ControlModelTestLinesChangedMessage
    {
        public IEnumerable <string> TestLineTypes { get; set; }
    }
}