using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Windsor;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    [Interceptor(typeof( MessageHandlerAspect ))]
    public abstract class SelkieInMemoryBusMessageHandlerAsync <T>
        where T : class
    {
        protected SelkieInMemoryBusMessageHandlerAsync(
            [NotNull] ISelkieLogger logger,
            [NotNull] ISelkieInMemoryBus bus)
        {
            Logger = logger;
            Bus = bus;

            string subscriptionId = GetType().FullName;

            bus.SubscribeAsync <T>(subscriptionId,
                                   Handle);
        }

        protected readonly ISelkieInMemoryBus Bus;
        protected readonly ISelkieLogger Logger;

        public abstract void Handle(T message);
    }
}