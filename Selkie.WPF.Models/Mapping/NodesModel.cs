using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class NodesModel : INodesModel
    {
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ISelkieLogger m_Logger;
        private readonly List <INodeModel> m_Nodes = new List <INodeModel>();

        public NodesModel([NotNull] ISelkieLogger logger,
                          [NotNull] ISelkieInMemoryBus bus,
                          [NotNull] ILinesSourceManager linesSourceManager)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;

            LoadNodes();

            bus.SubscribeAsync <ColonyLineResponseMessage>(GetType().FullName,
                                                           ColonyLineResponseHandler);
        }

        public IEnumerable <INodeModel> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        internal void ColonyLineResponseHandler(ColonyLineResponseMessage message)
        {
            m_Logger.Debug("Handling '{0}'...".Inject(message.GetType()));

            LoadNodes();
        }

        internal void LoadNodes()
        {
            m_Nodes.Clear();

            foreach ( ILine line in m_LinesSourceManager.Lines )
            {
                IEnumerable <INodeModel> models = CreateNodeModels(line);

                m_Nodes.AddRange(models);
            }

            m_Bus.Publish(new NodesModelChangedMessage());
        }

        internal IEnumerable <INodeModel> CreateNodeModels([NotNull] ILine line)
        {
            var models = new List <INodeModel>();

            int id = line.Id == 0
                         ? line.Id
                         : line.Id * 2;

            var start = new NodeModel(id,
                                      line.X1,
                                      line.Y1,
                                      line.AngleToXAxis);
            var finish = new NodeModel(id + 1,
                                       line.X2,
                                       line.Y2,
                                       line.AngleToXAxis);

            models.Add(start);
            models.Add(finish);

            return models;
        }
    }
}