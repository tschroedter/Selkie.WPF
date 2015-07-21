using System.Collections.Generic;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public class NodesModel : INodesModel
    {
        private readonly IBus m_Bus;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ILogger m_Logger;
        private readonly List <INodeModel> m_Nodes = new List <INodeModel>();

        public NodesModel(ILogger logger,
                          IBus bus,
                          ILinesSourceManager linesSourceManager)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;

            LoadNodes();

            bus.SubscribeHandlerAsync <ColonyLinesChangedMessage>(m_Logger,
                                                                  GetType().FullName,
                                                                  LinesChangedHandler);
        }

        public IEnumerable <INodeModel> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        internal void LinesChangedHandler(ColonyLinesChangedMessage message)
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

    // todo important write lines validator
    /*
             internal bool AreLineIdsValidate([NotNull] IEnumerable<ILine> lines)
        {
            ILine[] array = lines.ToArray();

            if (array.All(x => x.Id != 0))
            {
                m_Logger.Error("Couldn't find line with id = 0!");

                return false;
            }

            int minId = array.Min().Id;
            int maxId = array.Max().Id;

            for (int i = minId; i <= maxId; i++)
            {
                if (array.All(x => x.Id != i))
                {
                    m_Logger.Error("Duplicated line id {0} detected!".Inject(i));

                    return false;
                }
            }

            return true;
        }

     */
}