using System.Windows;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IDisplayLine
    {
        string Name { get; }
        int Id { get; }
        double X1 { get; }
        double Y1 { get; }
        double X2 { get; }
        double Y2 { get; }
        double DirectionAngle { get; }
        Point StartPoint { get; }
        Point EndPoint { get; }
    }
}