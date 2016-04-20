using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class EndNodeModelChangeHandler
        : BaseMapViewModelMessageHandler <EndNodeModelChangedMessage>
    {
        // todo testing
        private readonly INodeModelToDisplayNodeConverter m_Converter;
        private readonly IEndNodeModel m_Model;

        public EndNodeModelChangeHandler([NotNull] ISelkieLogger logger,
                                         [NotNull] ISelkieInMemoryBus bus,
                                         [NotNull] INodeModelToDisplayNodeConverter converter,
                                         [NotNull] IEndNodeModel model)
            : base(logger,
                   bus)
        {
            m_Converter = converter;
            m_Model = model;

            m_Converter.FillBrush = Brushes.Red;
            m_Converter.StrokeBrush = Brushes.DarkRed;
        }

        public override void Handle(EndNodeModelChangedMessage message)
        {
            m_Converter.NodeModel = m_Model.Node;
            m_Converter.Convert();


            MapViewModel.SetEndNode(m_Converter.DisplayNode);
        }
    }
}