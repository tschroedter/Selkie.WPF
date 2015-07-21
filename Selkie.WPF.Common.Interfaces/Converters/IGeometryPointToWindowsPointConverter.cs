using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Interfaces.Converters
{
    public interface IGeometryPointToWindowsPointConverter : IConverter
    {
        [NotNull]
        Point GeometryPoint { get; set; }

        System.Windows.Point Point { get; }
    }
}