using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Interfaces
{
    public interface INodeIdHelper
    {
        int NodeToLine(int node);
        bool IsForwardNode(int node);

        [CanBeNull]
        ILine GetLine(int lineId);

        int Reverse(int nodeId);
    }
}