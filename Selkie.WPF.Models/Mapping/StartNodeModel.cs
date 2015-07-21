using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class StartNodeModel
        : BaseNodeModel,
          IStartNodeModel
    {
        public StartNodeModel(ILogger logger,
                              IBus bus,
                              INodeIdHelper nodeIdHelper)
            : base(logger,
                   bus,
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