using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyLineResponseMessage
    {
        // todo add lines here
        public IEnumerable <ILine> Lines = new ILine[0];
    }
}