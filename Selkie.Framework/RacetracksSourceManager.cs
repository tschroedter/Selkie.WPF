using System.Linq;
using System.Text;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converter;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    // todo subscribe to message, testing
    [ProjectComponent(Lifestyle.Singleton)]
    public class RacetracksSourceManager : IRacetracksSourceManager
    {
        private readonly IBus m_Bus;
        private readonly IRacetracksDtoToRacetracksConverter m_Converter;
        private readonly ILogger m_Logger;
        private readonly object m_Padlock = new object();
        private IRacetracks m_Racetracks = RacetracksSource.Unknown;

        public RacetracksSourceManager([NotNull] ILogger logger,
                                       [NotNull] IBus bus,
                                       [NotNull] IRacetracksDtoToRacetracksConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_Converter = converter;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeHandlerAsync <ColonyRacetracksRequestMessage>(logger,
                                                                         subscriptionId,
                                                                         ColonyRacetracksGetHandler);

            m_Bus.SubscribeHandlerAsync <RacetracksChangedMessage>(logger,
                                                                   subscriptionId,
                                                                   RacetracksChangedHandler);

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
            SendColonyRacetracksChangedMessage();
        }

        internal void RacetracksChangedHandler([NotNull] RacetracksChangedMessage message)
        {
            lock ( m_Padlock )
            {
                m_Converter.Dto = message.Racetracks;
                m_Converter.Convert();

                m_Racetracks = m_Converter.Racetracks;
            }

            SendColonyRacetracksChangedMessage();
        }

        internal void SendColonyRacetracksChangedMessage()
        {
            IPath[][] forwardToForward = m_Racetracks.ForwardToForward;

            if ( !forwardToForward.Any() )
            {
                return;
            }

            m_Bus.PublishAsync(new ColonyRacetracksChangedMessage());

            LogRacetracks(forwardToForward);
        }

        private void LogRacetracks(IPath[][] forwardToForward)
        {
            m_Logger.Info("Racetracks");

            foreach ( IPath[] paths in forwardToForward )
            {
                var builder = new StringBuilder();

                foreach ( IPath path in paths )
                {
                    builder.Append(path + ", ");
                }
                m_Logger.Info(builder.ToString);
            }
        }
    }
}