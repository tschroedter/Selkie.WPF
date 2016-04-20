using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.ViewModels.Mapping.Handlers
{
    public class LinesModelChangedHandler
        : BaseMapViewModelMessageHandler <LinesModelChangedMessage>
    {
        private readonly ILinesModel m_LinesModel;

        public LinesModelChangedHandler([NotNull] ISelkieLogger logger,
                                        [NotNull] ISelkieInMemoryBus bus,
                                        [NotNull] ILinesModel linesModel)
            : base(logger,
                   bus)
        {
            m_LinesModel = linesModel;
        }

        public override void Handle(LinesModelChangedMessage message)
        {
            MapViewModel.SetLines(m_LinesModel.Lines);
        }
    }
}