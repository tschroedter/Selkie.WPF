using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface INodeIndexToLineConverter : IConverter
    {
        int NodeIndex { get; set; }
        ILine Line { get; }
    }
}