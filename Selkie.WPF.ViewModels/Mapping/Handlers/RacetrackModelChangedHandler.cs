using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class RacetrackModelChangedHandler
        : BaseMapViewModelMessageHandler <RacetrackModelChangedMessage>
    {
        public RacetrackModelChangedHandler([NotNull] ISelkieLogger logger,
                                            [NotNull] ISelkieInMemoryBus bus,
                                            [NotNull] IRacetrackPathsToFiguresConverter converter,
                                            [NotNull] IRacetrackModel model)
            : base(logger,
                   bus)
        {
            m_Converter = converter;
            m_Model = model;
        }

        private readonly IRacetrackPathsToFiguresConverter m_Converter;
        private readonly IRacetrackModel m_Model;

        public override void Handle(RacetrackModelChangedMessage message)
        {
            m_Converter.Paths = m_Model.Paths;
            m_Converter.Convert();

            MapViewModel.SetRacetracks(m_Converter.Figures);
        }
    }
}