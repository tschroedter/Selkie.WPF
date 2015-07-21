using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface ILineToLineNodeConverterToDisplayLineConverter
        : IConverter,
          IDisposable
    {
        [NotNull]
        IEnumerable <ILineToLineNodeConverter> Converters { get; set; }

        [NotNull]
        IEnumerable <IDisplayLine> DisplayLines { get; }
    }
}