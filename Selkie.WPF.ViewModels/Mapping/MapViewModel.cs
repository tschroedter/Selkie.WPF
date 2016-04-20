using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels.Mapping
{
    public class MapViewModel
        : ViewModel,
          IMapViewModel
    {
        private readonly IApplicationDispatcher m_Dispatcher;

        public MapViewModel([NotNull] ISelkieInMemoryBus bus,
                            [NotNull] IApplicationDispatcher dispatcher,
                            [NotNull] IMapViewModelMessageHandler[] handlers)
        {
            m_Dispatcher = dispatcher;

            foreach ( IMapViewModelMessageHandler handler in handlers )
            {
                handler.SetMapViewModel(this);
            }

            bus.PublishAsync(new LinesModelLinesRequestMessage());
        }

        public IEnumerable <PathFigureCollection> Racetracks { get; private set; }

        public IEnumerable <IDisplayLine> Lines { get; private set; }

        public IEnumerable <IDisplayLine> ShortestPath { get; private set; }

        public IEnumerable <IDisplayNode> PathDirections { get; private set; }

        public IEnumerable <IDisplayNode> Nodes { get; private set; }

        public IDisplayNode StartNode { get; private set; }

        public IDisplayNode EndNode { get; private set; }

        public void SetshortestPath(IEnumerable <IDisplayLine> shortestPath)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         ShortestPath = shortestPath;

                                         NotifyPropertyChanged("ShortestPath");
                                     });
        }

        public void SetLines(IEnumerable <IDisplayLine> lines)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         var list = new List <IDisplayLine>();
                                         list.AddRange(lines);

                                         Lines = list;

                                         NotifyPropertyChanged("Lines");
                                     });
        }

        public void SetNodes(IEnumerable <IDisplayNode> displayNodes)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         var list = new List <IDisplayNode>();
                                         list.AddRange(displayNodes);

                                         Nodes = list;

                                         NotifyPropertyChanged("Nodes");
                                     });
        }

        public void SetStartNode(IDisplayNode displayNode)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         StartNode = displayNode;

                                         NotifyPropertyChanged("StartNode");
                                     });
        }

        public void SetEndNode(IDisplayNode displayNode)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         EndNode = displayNode;

                                         NotifyPropertyChanged("EndNode");
                                     });
        }

        public void SetRacetracks(IEnumerable <PathFigureCollection> figures)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         var list = new List <PathFigureCollection>();
                                         list.AddRange(figures);

                                         Racetracks = list;

                                         NotifyPropertyChanged("Racetracks");
                                     });
        }

        public void SetDirections(IEnumerable <IDisplayNode> displayNodes)
        {
            m_Dispatcher.BeginInvoke(() =>
                                     {
                                         var list = new List <IDisplayNode>();
                                         list.AddRange(displayNodes);

                                         PathDirections = list;

                                         NotifyPropertyChanged("PathDirections");
                                     });
        }
    }
}