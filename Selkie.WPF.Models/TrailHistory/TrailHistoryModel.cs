using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.TrailHistory
{
    public sealed class TrailHistoryModel
        : ITrailHistoryModel,
          IDisposable
    {
        private readonly ISelkieInMemoryBus m_MemoryBus;

        private readonly SortedDictionary <int, ITrailDetails> m_Dictionary =
            new SortedDictionary <int, ITrailDetails>();

        private readonly ITrailDetailsFactory m_Factory;
        private readonly ISelkieLogger m_Logger;
        private ITrailDetails m_FirstTrailDetails = TrailDetails.Unknown;
        private List <ITrailDetails> m_TrailDetails = new List <ITrailDetails>();

        public TrailHistoryModel([NotNull] ISelkieLogger logger,
                                 [NotNull] ISelkieBus bus,
                                 [NotNull] ISelkieInMemoryBus memoryBus,
                                 [NotNull] ITrailDetailsFactory factory)
        {
            m_Logger = logger;
            m_MemoryBus = memoryBus;
            m_Factory = factory;

            string subscriptionId = GetType().FullName;

            bus.SubscribeAsync <ColonyBestTrailMessage>(subscriptionId,
                                                        ColonyBestTrailHandler);

            bus.SubscribeAsync <ColonyStartRequestMessage>(subscriptionId,
                                                           ColonyStartRequestHandler);
        }

        public void Dispose()
        {
            ReleaseTrailDetails();
        }

        public IEnumerable <ITrailDetails> Trails
        {
            get
            {
                return m_TrailDetails;
            }
        }

        public ITrailDetails FirsTrailDetails
        {
            get
            {
                return m_FirstTrailDetails;
            }
        }

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            Update(message);
        }

        internal void ColonyStartRequestHandler(ColonyStartRequestMessage message)
        {
            Started();
        }

        internal void Started()
        {
            lock ( this )
            {
                m_Logger.Info("Started called!");

                ReleaseTrailDetails();

                m_TrailDetails.Clear();
                m_Dictionary.Clear();

                m_FirstTrailDetails = TrailDetails.Unknown;
            }
        }

        internal void Update(ColonyBestTrailMessage message)
        {
            lock ( this )
            {
                ITrailDetails details = CreateTrailDetails(message);

                if ( m_Dictionary.ContainsKey(details.Interation) )
                {
                    m_Dictionary.Remove(details.Interation);
                }

                m_Dictionary.Add(details.Interation,
                                 details);

                m_TrailDetails = CreateTrailDetailsList(m_Dictionary.Values.ToArray());

                m_Logger.Info("Update called! (Count: {0})".Inject(m_Dictionary.Count));

                m_MemoryBus.PublishAsync(new TrailHistoryModelChangedMessage());
            }
        }

        internal List <ITrailDetails> CreateTrailDetailsList(ITrailDetails[] values)
        {
            var list = new List <ITrailDetails>();

            ITrailDetails firstDetails = TrailDetails.Unknown;

            for ( var i = 0 ; i < values.Length ; i++ )
            {
                ITrailDetails current = values [ i ];
                ITrailDetails trailDetails;

                if ( i == 0 )
                {
                    firstDetails = current;
                    trailDetails = current;
                }
                else
                {
                    trailDetails = CreateOtherTrailDetails(firstDetails,
                                                           current);
                }

                list.Add(trailDetails);
            }

            return list;
        }

        internal ITrailDetails CreateOtherTrailDetails([NotNull] ITrailDetails relativeTo,
                                                       [NotNull] ITrailDetails trailDetails)
        {
            double delta = relativeTo.Length - trailDetails.Length;
            double percent = delta * 100.0 / relativeTo.Length;

            ITrailDetails details = m_Factory.Create(trailDetails.Interation,
                                                     trailDetails.Trail,
                                                     trailDetails.Length,
                                                     delta,
                                                     percent,
                                                     trailDetails.Type,
                                                     trailDetails.Alpha,
                                                     trailDetails.Beta,
                                                     trailDetails.Gamma);

            return details;
        }

        internal ITrailDetails CreateTrailDetails(ColonyBestTrailMessage message)
        {
            ITrailDetails details = m_Factory.Create(message.Iteration,
                                                     message.Trail,
                                                     message.Length,
                                                     0.0,
                                                     0.0,
                                                     message.Type,
                                                     message.Alpha,
                                                     message.Beta,
                                                     message.Gamma);

            m_FirstTrailDetails = details;
            return details;
        }

        internal void ReleaseTrailDetails()
        {
            ITrailDetails[] trailDetails = m_TrailDetails.ToArray();

            foreach ( ITrailDetails trailDetail in trailDetails )
            {
                m_Factory.Release(trailDetail);
            }
        }
    }
}