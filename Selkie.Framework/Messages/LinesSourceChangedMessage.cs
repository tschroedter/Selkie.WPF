using System.Collections.Generic;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Messages
{
    public class LinesSourceChangedMessage
    {
        public IEnumerable <ILine> Lines { get; set; }
    }
}