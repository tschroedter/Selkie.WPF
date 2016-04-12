using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Models.Interfaces
{
    public interface ILinesToNodesConverter
    {
        IEnumerable <IAntSettingsNode> Nodes { get; }

        void Convert([NotNull] IAntSettingsNodeFactory antSettingsNodeFactory,
                     [NotNull] IEnumerable <ILine> lines);
    }
}