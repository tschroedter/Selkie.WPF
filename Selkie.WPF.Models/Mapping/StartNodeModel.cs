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
        public StartNodeModel([NotNull] ISelkieInMemoryBus bus,
                              [NotNull] INodeIdHelper nodeIdHelper)
            : base(bus,
                   nodeIdHelper)
        {
        }

        public override int DetermineNodeId(IEnumerable <int> trail)
        {
            return trail.FirstOrDefault();
        }

        public override void SendMessage()
        {
            Bus.Publish(new StartNodeModelChangedMessage());
        }
    }
}