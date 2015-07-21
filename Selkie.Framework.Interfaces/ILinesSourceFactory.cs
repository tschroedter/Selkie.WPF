using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;

namespace Selkie.Framework.Interfaces
{
    public interface ILinesSourceFactory : ITypedFactory
    {
        ILinesSource Create([NotNull] IEnumerable <ILine> lines);
        void Release([NotNull] ILinesSource linesSource);
    }
}