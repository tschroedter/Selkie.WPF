using System.Windows;
using System.Windows.Media;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IDisplayNode
    {
        int Id { get; }
        string Name { get; }
        Point OriginalCentrePoint { get; }
        Point CentrePoint { get; }
        Point Position { get; }
        double X { get; }
        double Y { get; }
        double Width { get; }
        double Height { get; }
        double Radius { get; }
        SolidColorBrush Stroke { get; }
        SolidColorBrush Fill { get; }
        double StrokeThickness { get; }
        double DirectionAngle { get; }
        bool IsUnknown { get; }
        bool IsVisible { get; }
    }
}