using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class RacetracksSourceManager : IRacetracksSourceManager
    {
        private readonly ISelkieBus m_Bus;
        private readonly IRacetracksDtoToRacetracksConverter m_Converter;
        private readonly ISelkieLogger m_Logger;
        private readonly object m_Padlock = new object();
        private IRacetracks m_Racetracks = RacetracksSource.Unknown;

        public RacetracksSourceManager([NotNull] ISelkieLogger logger,
                                       [NotNull] ISelkieBus bus,
                                       [NotNull] IRacetracksDtoToRacetracksConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_Converter = converter;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <ColonyRacetracksRequestMessage>(subscriptionId,
                                                                  ColonyRacetracksGetHandler);

            m_Bus.SubscribeAsync <RacetracksResponseMessage>(subscriptionId,
                                                             RacetracksResponseHandler);

            m_Bus.PublishAsync(new RacetracksGetMessage()); // todo rename message to ...Request...
        }

        public IRacetracks Racetracks
        {
            get
            {
                return m_Racetracks;
            }
        }

        internal void ColonyRacetracksGetHandler(ColonyRacetracksRequestMessage message)
        {
            SendColonyRacetracksResponseMessage();
        }

        internal void RacetracksResponseHandler([NotNull] RacetracksResponseMessage message)
        {
            lock ( m_Padlock )
            {
                m_Converter.Dto = message.Racetracks;
                m_Converter.Convert();

                m_Racetracks = m_Converter.Racetracks;
            }

            SendColonyRacetracksResponseMessage();
        }

        internal void SendColonyRacetracksResponseMessage()
        {
            IPath[][] forwardToForward = m_Racetracks.ForwardToForward;

            if ( !forwardToForward.Any() )
            {
                return;
            }

            m_Bus.PublishAsync(new ColonyRacetracksResponseMessage());

            LogRacetracks(forwardToForward);
        }

        private void LogRacetracks(IEnumerable <IPath[]> forwardToForward)
        {
            m_Logger.Info("Racetracks");

            foreach ( IPath[] paths in forwardToForward )
            {
                var builder = new StringBuilder();

                foreach ( IPath path in paths )
                {
                    builder.Append(path + ", ");
                }
                m_Logger.Info(builder.ToString());
            }
        }
    }
}