using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class ShortestPathDirectionModelChangedHandler
        : BaseMapViewModelMessageHandler <ShortestPathDirectionModelChangedMessage>
    {
        private readonly INodesToDisplayNodesConverter m_Converter;
        private readonly IShortestPathDirectionModel m_Model;

        public ShortestPathDirectionModelChangedHandler([NotNull] ISelkieLogger logger,
                                                        [NotNull] ISelkieInMemoryBus bus,
                                                        [NotNull] INodesToDisplayNodesConverter converter,
                                                        [NotNull] IShortestPathDirectionModel model)
            : base(logger,
                   bus)
        {
            m_Converter = converter;
            m_Model = model;
        }

        public override void Handle(ShortestPathDirectionModelChangedMessage message)
        {
            m_Converter.NodeModels = m_Model.Nodes;
            m_Converter.Convert();

            MapViewModel.SetDirections(m_Converter.DisplayNodes);
        }
    }
}