using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class AntSettingsSourceManager : IAntSettingsSourceManager
    {
        private const bool UseRandomStartNode = false;
        private const int FixedStartNodeZero = 0;
        private readonly ISelkieBus m_Bus;
        private readonly IAntSettingsSourceFactory m_Factory;

        public AntSettingsSourceManager([NotNull] ISelkieInMemoryBus bus,
                                        [NotNull] IAntSettingsSourceFactory factory)
        {
            m_Bus = bus;
            m_Factory = factory;

            Source = m_Factory.Create(UseRandomStartNode,
                                      FixedStartNodeZero);

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <ColonyAntSettingsRequestMessage>(subscriptionId,
                                                                   ColonyAntSettingsRequestHandler);

            m_Bus.SubscribeAsync <ColonyAntSettingsSetMessage>(subscriptionId,
                                                               ColonyAntSettingsSetHandler);

            m_Bus.SubscribeAsync <ColonyLineResponseMessage>(subscriptionId,
                                                             ColonyLineResponseHandler);

            SendResponseMessage();
        }

        public IAntSettingsSource Source { get; private set; }

        internal void ColonyAntSettingsSetHandler(ColonyAntSettingsSetMessage message)
        {
            UpdateSource(message.IsFixedStartNode,
                         message.FixedStartNode);

            SendResponseMessage();
        }

        private void UpdateSource(bool isFixedStartNode,
                                  int fixedStartNode)
        {
            IAntSettingsSource oldSource = Source;

            Source = m_Factory.Create(isFixedStartNode,
                                      fixedStartNode);

            m_Factory.Release(oldSource);
        }

        internal void ColonyAntSettingsRequestHandler(ColonyAntSettingsRequestMessage message)
        {
            SendResponseMessage();
        }

        internal void ColonyLineResponseHandler(ColonyLineResponseMessage message)
        {
            UpdateSource(false,
                         0);

            SendResponseMessage();
        }

        private void SendResponseMessage()
        {
            var reply = new ColonyAntSettingsResponseMessage
                        {
                            IsFixedStartNode = Source.IsFixedStartNode,
                            FixedStartNode = Source.FixedStartNode
                        };

            m_Bus.PublishAsync(reply);
        }
    }
}