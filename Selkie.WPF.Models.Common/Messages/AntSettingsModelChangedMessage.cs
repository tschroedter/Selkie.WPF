using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Common.Messages
{
    public class AntSettingsModelChangedMessage
    {
        public AntSettingsModelChangedMessage(bool isFixedStartNode,
                                              int fixedStartNode,
                                              [NotNull] IEnumerable <IAntSettingsNode> nodes)
        {
            IsFixedStartNode = isFixedStartNode;
            FixedStartNode = fixedStartNode;
            Nodes = nodes;
        }

        public bool IsFixedStartNode { get; private set; }
        public int FixedStartNode { get; private set; }

        [NotNull]
        public IEnumerable <IAntSettingsNode> Nodes { get; private set; }
    }
}