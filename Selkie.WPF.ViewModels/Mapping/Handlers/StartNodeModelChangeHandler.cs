using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class StartNodeModelChangeHandler
        : BaseMapViewModelMessageHandler <StartNodeModelChangedMessage>
    {
        private readonly INodeModelToDisplayNodeConverter m_Converter;
        private readonly IStartNodeModel m_Model;

        public StartNodeModelChangeHandler([NotNull] ISelkieLogger logger,
                                           [NotNull] ISelkieInMemoryBus bus,
                                           [NotNull] INodeModelToDisplayNodeConverter converter,
                                           [NotNull] IStartNodeModel model)
            : base(logger,
                   bus)
        {
            m_Converter = converter;
            m_Model = model;
        }

        public override void Handle(StartNodeModelChangedMessage message)
        {
            m_Converter.NodeModel = m_Model.Node;
            m_Converter.Convert();


            MapViewModel.SetStartNode(m_Converter.DisplayNode);
        }
    }
}