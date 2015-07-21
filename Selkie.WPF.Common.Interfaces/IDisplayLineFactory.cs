using Selkie.Common;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IDisplayLineFactory : ITypedFactory
    {
        IDisplayLine Create(ILine line);

        IDisplayLine Create(ILine line,
                            Constants.LineDirection direction);

        void Release(IDisplayLine displayLine);
    }
}