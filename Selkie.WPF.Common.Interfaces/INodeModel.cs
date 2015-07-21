using Selkie.Geometry.Primitives;

namespace Selkie.WPF.Common.Interfaces
{
    public interface INodeModel
    {
        int Id { get; }
        double X { get; }
        double Y { get; }
        Angle DirectionAngle { get; }
        bool IsUnknown { get; }
    }
}