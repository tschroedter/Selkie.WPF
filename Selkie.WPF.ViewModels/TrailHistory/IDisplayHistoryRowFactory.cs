using System.Collections.Generic;
using Selkie.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.TrailHistory
{
    public interface IDisplayHistoryRowFactory : ITypedFactory
    {
        IDisplayHistoryRow Create(int interation,
                                  IEnumerable <int> trail,
                                  double length,
                                  double lengthDelta,
                                  double lengthDeltaInPercent,
                                  double alpha,
                                  double beta,
                                  double gamma,
                                  string type);

        void Release(IDisplayHistoryRow displayHistoryRow);
    }
}