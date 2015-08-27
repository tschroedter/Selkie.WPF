using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.Mapping
{
    // todo a god class, but don't know how to split this up yet
    public class MapViewModel
        : ViewModel,
          IMapViewModel
    {
        private readonly INodesToDisplayNodesConverter m_ConverterDirections;
        private readonly INodeModelToDisplayNodeConverter m_ConverterEndNodeModel;
        private readonly ILineNodeToDisplayLineNodeConverter m_ConverterNodes;
        private readonly IRacetrackPathsToFiguresConverter m_ConverterRacetrack;
        private readonly INodeModelToDisplayNodeConverter m_ConverterStartNodeModel;
        private readonly IApplicationDispatcher m_Dispatcher;
        private readonly IEndNodeModel m_EndNodeModel;
        private readonly ILinesModel m_LinesModel;
        private readonly INodesModel m_NodesModel;
        private readonly IRacetrackModel m_RacetrackModel;
        private readonly IShortestPathDirectionModel m_ShortestPathDirectionModel;
        private readonly IShortestPathModel m_ShortestPathModel;
        private readonly IStartNodeModel m_StartNodeModel;
        private List <IDisplayNode> m_Directions;
        private List <IDisplayLine> m_Lines;
        private List <IDisplayNode> m_Nodes;
        private List <PathFigureCollection> m_Racetrack;
        private List <IDisplayLine> m_ShortestPath;

        public MapViewModel([NotNull] ISelkieInMemoryBus bus,
                            [NotNull] IApplicationDispatcher dispatcher,
                            [NotNull] ILineNodeToDisplayLineNodeConverter converterNodes,
                            [NotNull] INodeModelToDisplayNodeConverter converterStartNodeModel,
                            [NotNull] INodeModelToDisplayNodeConverter converterEndNodeModel,
                            [NotNull] IRacetrackPathsToFiguresConverter converterRacetrack,
                            [NotNull] INodesToDisplayNodesConverter converterDirections,
                            [NotNull] ILinesModel linesModel,
                            [NotNull] INodesModel nodesModel,
                            [NotNull] IStartNodeModel startNodeModel,
                            [NotNull] IEndNodeModel endNodeModel,
                            [NotNull] IShortestPathModel shortestPathModel,
                            [NotNull] IShortestPathDirectionModel shortestPathDirectionModel,
                            [NotNull] IRacetrackModel racetrackModel)
        {
            m_Dispatcher = dispatcher;
            m_ConverterNodes = converterNodes;
            m_ConverterStartNodeModel = converterStartNodeModel;
            m_ConverterEndNodeModel = converterEndNodeModel;
            m_ConverterRacetrack = converterRacetrack;
            m_ConverterDirections = converterDirections;
            m_LinesModel = linesModel;
            m_NodesModel = nodesModel;
            m_StartNodeModel = startNodeModel;
            m_EndNodeModel = endNodeModel;
            m_ShortestPathModel = shortestPathModel;
            m_ShortestPathDirectionModel = shortestPathDirectionModel;
            m_RacetrackModel = racetrackModel;

            m_ConverterEndNodeModel.FillBrush = Brushes.Red;
            m_ConverterEndNodeModel.StrokeBrush = Brushes.DarkRed;

            string subscriptionId = GetType().FullName; // todo ICosume <T, T1,...>
            bus.SubscribeAsync <ShortestPathModelChangedMessage>(subscriptionId,
                                                                 ShortestPathModelChangedHandler);
            bus.SubscribeAsync <LinesModelChangedMessage>(subscriptionId,
                                                          LinesModelChangedMessageHandler);
            bus.SubscribeAsync <NodesModelChangedMessage>(subscriptionId,
                                                          NodesModelChangedHandler);
            bus.SubscribeAsync <StartNodeModelChangedMessage>(subscriptionId,
                                                              StartNodeModelChangedHandler);
            bus.SubscribeAsync <EndNodeModelChangedMessage>(subscriptionId,
                                                            EndNodeModelChangedHandler);
            bus.SubscribeAsync <RacetrackModelChangedMessage>(subscriptionId,
                                                              RacetrackModelChangedHandler);
            bus.SubscribeAsync <ShortestPathDirectionModelChangedMessage>(subscriptionId,
                                                                          ShortestPathDirectionModelChangedHandler);

            bus.PublishAsync(new LinesModelLinesRequestMessage());
        }

        public IEnumerable <PathFigureCollection> Racetracks
        {
            get
            {
                return m_Racetrack;
            }
        }

        public IEnumerable <IDisplayLine> Lines
        {
            get
            {
                return m_Lines;
            }
        }

        public IEnumerable <IDisplayLine> ShortestPath
        {
            get
            {
                return m_ShortestPath;
            }
        }

        public IEnumerable <IDisplayNode> PathDirections
        {
            get
            {
                return m_Directions;
            }
        }

        public IEnumerable <IDisplayNode> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        public IDisplayNode StartNode { get; private set; }
        public IDisplayNode EndNode { get; private set; }

        internal void ShortestPathDirectionModelChangedHandler(ShortestPathDirectionModelChangedMessage message)
        {
            m_ConverterDirections.NodeModels = m_ShortestPathDirectionModel.Nodes;
            m_ConverterDirections.Convert();

            m_Dispatcher.BeginInvoke(UpdatePathDirections);
        }

        internal void ShortestPathModelChangedHandler(ShortestPathModelChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(UpdateShortestPath);
        }

        internal void UpdateShortestPath()
        {
            m_ShortestPath = new List <IDisplayLine>();
            m_ShortestPath.AddRange(m_ShortestPathModel.Path);

            NotifyPropertyChanged("ShortestPath");
        }

        internal void LinesModelChangedMessageHandler(LinesModelChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(UpdateLines);
        }

        internal void UpdateLines()
        {
            m_Lines = new List <IDisplayLine>();
            m_Lines.AddRange(m_LinesModel.Lines);

            NotifyPropertyChanged("Lines");
        }

        internal void NodesModelChangedHandler(NodesModelChangedMessage message)
        {
            m_ConverterNodes.NodeModels = m_NodesModel.Nodes;
            m_ConverterNodes.Convert();

            m_Dispatcher.BeginInvoke(UpdateNodes);
        }

        internal void StartNodeModelChangedHandler(StartNodeModelChangedMessage message)
        {
            m_ConverterStartNodeModel.NodeModel = m_StartNodeModel.Node;
            m_ConverterStartNodeModel.Convert();

            m_Dispatcher.BeginInvoke(UpdateStartNode);
        }

        internal void EndNodeModelChangedHandler(EndNodeModelChangedMessage message)
        {
            m_ConverterEndNodeModel.NodeModel = m_EndNodeModel.Node;
            m_ConverterEndNodeModel.Convert();

            m_Dispatcher.BeginInvoke(UpdateEndNode);
        }

        internal void RacetrackModelChangedHandler(RacetrackModelChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(UpdateRacetrack);
        }

        internal void UpdateNodes()
        {
            m_Nodes = new List <IDisplayNode>();
            m_Nodes.AddRange(m_ConverterNodes.DisplayNodes);

            NotifyPropertyChanged("Nodes");
        }

        internal void UpdateStartNode()
        {
            StartNode = m_ConverterStartNodeModel.DisplayNode;

            NotifyPropertyChanged("StartNode");
        }

        internal void UpdateEndNode()
        {
            EndNode = m_ConverterEndNodeModel.DisplayNode;

            NotifyPropertyChanged("EndNode");
        }

        internal void UpdateRacetrack()
        {
            m_ConverterRacetrack.Paths = m_RacetrackModel.Paths;
            m_ConverterRacetrack.Convert();

            m_Racetrack = new List <PathFigureCollection>();
            m_Racetrack.AddRange(m_ConverterRacetrack.Figures);

            NotifyPropertyChanged("Racetracks");
        }

        internal void UpdatePathDirections()
        {
            m_Directions = new List <IDisplayNode>();
            m_Directions.AddRange(m_ConverterDirections.DisplayNodes);

            NotifyPropertyChanged("PathDirections");
        }
    }
}