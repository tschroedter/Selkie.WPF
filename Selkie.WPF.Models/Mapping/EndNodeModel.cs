using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class EndNodeModel
        : BaseNodeModel,
          IEndNodeModel
    {
        public EndNodeModel([NotNull] ISelkieInMemoryBus bus,
                            [NotNull] INodeModelCreator nodeModelCreator)
            : base(bus,
                   nodeModelCreator)
        {
        }

        public override int DetermineNodeId(IEnumerable <int> trail)
        {
            int nodeId = trail.LastOrDefault();

            nodeId = Helper.Reverse(nodeId);

            return nodeId;
        }

        public override void SendMessage()
        {
            Bus.Publish(new EndNodeModelChangedMessage());
        }
    }
}