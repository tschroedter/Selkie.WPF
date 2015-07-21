using System.Windows.Media;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface INodeModelToDisplayNodeConverter : IConverter
    {
        INodeModel NodeModel { get; set; }
        IDisplayNode DisplayNode { get; }
        SolidColorBrush FillBrush { get; set; }
        SolidColorBrush StrokeBrush { get; set; }
    }
}