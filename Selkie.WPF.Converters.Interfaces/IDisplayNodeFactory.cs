using System.Windows.Media;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IDisplayNodeFactory : ITypedFactory
    {
        IDisplayNode Create(int id,
                            double x,
                            double y,
                            double directionAngle,
                            double radius,
                            SolidColorBrush fill,
                            SolidColorBrush stroke,
                            double strokeThickness);

        void Release(IDisplayNode displayNode);
    }
}