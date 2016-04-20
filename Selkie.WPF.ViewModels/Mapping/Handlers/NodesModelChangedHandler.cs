using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class NodesModelChangedHandler
        : BaseMapViewModelMessageHandler <NodesModelChangedMessage>
    {
        private readonly ILineNodeToDisplayLineNodeConverter m_Converter;
        private readonly INodesModel m_NodesModel;

        public NodesModelChangedHandler([NotNull] ISelkieLogger logger,
                                        [NotNull] ISelkieInMemoryBus bus,
                                        [NotNull] ILineNodeToDisplayLineNodeConverter converter,
                                        [NotNull] INodesModel nodesModel)
            : base(logger,
                   bus)
        {
            m_NodesModel = nodesModel;
            m_Converter = converter;
        }

        public override void Handle(NodesModelChangedMessage message)
        {
            m_Converter.NodeModels = m_NodesModel.Nodes;
            m_Converter.Convert();

            MapViewModel.SetNodes(m_Converter.DisplayNodes);
        }
    }
}