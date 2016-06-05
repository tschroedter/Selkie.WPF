using Selkie.Common;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface INodeIndexHelper
    {
        bool IsValidIndex(int index);
        Constants.LineDirection NodeIndexToDirection(int index);
        ILine NodeIndexToLine(int index);
    }
}