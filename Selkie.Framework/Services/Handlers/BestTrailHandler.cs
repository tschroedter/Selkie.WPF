using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.Framework.Common.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Services.Handlers
{
    [ProjectComponent(Lifestyle.Startable)]
    public class BestTrailHandler : BaseHandler <BestTrailMessage>
    {
        public BestTrailHandler([NotNull] ILogger logger,
                                [NotNull] IBus bus)
            : base(logger,
                   bus)
        {
        }

        internal override void Handle(BestTrailMessage message)
        {
            Bus.PublishAsync(new ColonyBestTrailMessage
                             {
                                 Alpha = message.Alpha,
                                 Beta = message.Beta,
                                 Gamma = message.Gamma,
                                 Iteration = message.Iteration,
                                 Trail = message.Trail,
                                 Type = message.Type,
                                 Length = message.Length
                             });
        }
    }
}