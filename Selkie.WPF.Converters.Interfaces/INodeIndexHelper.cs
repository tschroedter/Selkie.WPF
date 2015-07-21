using Selkie.Common;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface INodeIndexHelper
    {
        ILine NodeIndexToLine(int index);
        Constants.LineDirection NodeIndexToDirection(int index);
        bool IsValidIndex(int index);
    }
}