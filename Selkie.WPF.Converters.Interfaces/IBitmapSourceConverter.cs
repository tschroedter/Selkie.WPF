using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IBitmapSourceConverter : IConverter
    {
        [NotNull]
        List <List <int>> Data { get; set; }

        ImageSource ImageSource { get; }
    }
}