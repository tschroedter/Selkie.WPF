using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.TrailHistory.Converters;

namespace Selkie.WPF.ViewModels.TrailHistory
{
    public class TrailHistoryViewModel
        : ViewModel,
          ITrailHistoryViewModel
    {
        private readonly IApplicationDispatcher m_ApplicationDispatcher;
        private readonly ITrailDetailsToDisplayHistoryRowsConverter m_Converter;
        private readonly ITrailHistoryModel m_TrailHistoryModel;
        private List <IDisplayHistoryRow> m_Rows = new List <IDisplayHistoryRow>();

        public TrailHistoryViewModel([NotNull] ISelkieInMemoryBus bus,
                                     [NotNull] IApplicationDispatcher applicationDispatcher,
                                     [NotNull] ITrailDetailsToDisplayHistoryRowsConverter converter,
                                     [NotNull] ITrailHistoryModel trailHistoryModel)
        {
            m_ApplicationDispatcher = applicationDispatcher;
            m_Converter = converter;
            m_TrailHistoryModel = trailHistoryModel;

            bus.SubscribeAsync <TrailHistoryModelChangedMessage>(GetType().ToString(),
                                                                 TrailHistoryModelChangedHandler);
        }

        public IEnumerable <IDisplayHistoryRow> Rows
        {
            get
            {
                return m_Rows;
            }
        }

        internal void TrailHistoryModelChangedHandler(TrailHistoryModelChangedMessage message)
        {
            m_Converter.Trails = m_TrailHistoryModel.Trails;
            m_Converter.Convert();

            m_ApplicationDispatcher.BeginInvoke(() => Update(m_Converter.DisplayHistoryRows));
        }

        internal void Update(IEnumerable <IDisplayHistoryRow> displayHistoryRows)
        {
            m_Rows = new List <IDisplayHistoryRow>();
            m_Rows.AddRange(displayHistoryRows);

            NotifyPropertyChanged("Rows");
        }
    }
}