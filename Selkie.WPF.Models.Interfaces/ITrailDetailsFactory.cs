using System.Collections.Generic;
using Selkie.Windsor;

namespace Selkie.WPF.Models.Interfaces
{
    public interface ITrailDetailsFactory : ITypedFactory
    {
        ITrailDetails Create(int interation,
                             IEnumerable <int> trail,
                             double length,
                             double lengthDelta,
                             double lengthDeltaInPercent,
                             string type,
                             double alpha,
                             double beta,
                             double gamma);

        void Release(ITrailDetails name);
    }
}