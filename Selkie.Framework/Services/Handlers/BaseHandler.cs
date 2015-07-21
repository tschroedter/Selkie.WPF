using Castle.Core;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework.Services.Handlers
{
    // todo use EasyNetQ IConsume https://github.com/EasyNetQ/EasyNetQ/wiki/Auto-Subscriber
    public abstract class BaseHandler <T> : IStartable
        where T : class
    {
        private readonly IBus m_Bus;
        private readonly ILogger m_Logger;

        protected BaseHandler([NotNull] ILogger logger,
                              [NotNull] IBus bus)
        {
            m_Logger = logger;
            m_Bus = bus;

            m_Logger.Info("Created handler for <{0}>!".Inject(GetType().FullName));
        }

        [NotNull]
        protected IBus Bus
        {
            get
            {
                return m_Bus;
            }
        }

        public void Start()
        {
            m_Logger.Info("Subscribing to message <{0}>...".Inject(typeof ( T )));

            m_Bus.SubscribeHandlerAsync <T>(m_Logger,
                                            GetType().FullName,
                                            Handle);
        }

        public void Stop()
        {
            m_Logger.Info("...stopped subscribing to message <{0}>!".Inject(typeof ( T )));
        }

        internal abstract void Handle([NotNull] T message);
    }
}