using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface ILineNodeToDisplayLineNodeConverter : IConverter
    {
        [NotNull]
        IEnumerable <INodeModel> NodeModels { get; set; }

        [NotNull]
        IEnumerable <IDisplayNode> DisplayNodes { get; }
    }
}