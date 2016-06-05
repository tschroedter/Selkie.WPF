using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Interfaces
{
    public interface INodeIdHelper
    {
        [CanBeNull]
        ILine GetLine(int lineId);

        bool IsForwardNode(int node);
        int NodeToLine(int node);

        int Reverse(int nodeId);
    }
}