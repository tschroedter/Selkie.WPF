using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.TrailHistory.Converters
{
    public sealed class TrailDetailsToDisplayDisplayHistoryRowsConverter
        : ITrailDetailsToDisplayHistoryRowsConverter,
          IDisposable
    {
        private readonly IDisposer m_Disposer;
        private readonly IDisplayHistoryRowFactory m_Factory;
        private readonly List <IDisplayHistoryRow> m_Rows = new List <IDisplayHistoryRow>();
        private IEnumerable <ITrailDetails> m_TrailDetails = new List <ITrailDetails>();

        public TrailDetailsToDisplayDisplayHistoryRowsConverter([NotNull] IDisposer disposer,
                                                                [NotNull] IDisplayHistoryRowFactory factory)
        {
            m_Disposer = disposer;
            m_Factory = factory;

            m_Disposer.AddResource(ReleaseDisplayHistoryRows);
        }

        public void Dispose()
        {
            m_Disposer.Dispose();
        }

        public void Convert()
        {
            ReleaseDisplayHistoryRows();
            AddDisplayHistoryRows();
        }

        public IEnumerable <ITrailDetails> Trails
        {
            get
            {
                return m_TrailDetails;
            }
            set
            {
                m_TrailDetails = value;
            }
        }

        public IEnumerable <IDisplayHistoryRow> DisplayHistoryRows
        {
            get
            {
                return m_Rows;
            }
        }

        internal void AddDisplayHistoryRows()
        {
            foreach ( ITrailDetails details in m_TrailDetails )
            {
                IDisplayHistoryRow row = CreateDisplayHistoryRow(details);

                m_Rows.Add(row);
            }
        }

        internal IDisplayHistoryRow CreateDisplayHistoryRow(ITrailDetails trailDetails)
        {
            IDisplayHistoryRow row = m_Factory.Create(trailDetails.Interation,
                                                      trailDetails.Trail,
                                                      trailDetails.Length,
                                                      trailDetails.LengthDelta,
                                                      trailDetails.LengthDeltaInPercent,
                                                      trailDetails.Alpha,
                                                      trailDetails.Beta,
                                                      trailDetails.Gamma,
                                                      trailDetails.Type);

            return row;
        }

        internal void ReleaseDisplayHistoryRows()
        {
            IDisplayHistoryRow[] rows = m_Rows.ToArray();

            foreach ( IDisplayHistoryRow row in rows )
            {
                m_Factory.Release(row);
            }

            m_Rows.Clear();
        }
    }
}