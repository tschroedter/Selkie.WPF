using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class StartNodeModel
        : BaseNodeModel,
          IStartNodeModel
    {
        public StartNodeModel([NotNull] ISelkieBus bus,
                              [NotNull] ISelkieInMemoryBus memoryBus,
                              [NotNull] INodeIdHelper nodeIdHelper)
            : base(bus,
                   memoryBus,
                   nodeIdHelper)
        {
        }

        public override int DetermineNodeId(IEnumerable <int> trail)
        {
            return trail.FirstOrDefault();
        }

        public override void SendMessage()
        {
            MemoryBus.Publish(new StartNodeModelChangedMessage());
        }
    }
}