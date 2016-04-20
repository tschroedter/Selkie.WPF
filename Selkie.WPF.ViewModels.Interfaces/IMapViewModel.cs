using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.ViewModels.Interfaces
{
    public interface IMapViewModel : IViewModel
    {
        void SetshortestPath([NotNull] IEnumerable <IDisplayLine> shortestPath);
        void SetLines([NotNull] IEnumerable <IDisplayLine> lines);
        void SetNodes([NotNull] IEnumerable <IDisplayNode> displayNodes);
        void SetStartNode([NotNull] IDisplayNode displayNode);
        void SetEndNode([NotNull] IDisplayNode displayNode);
        void SetRacetracks([NotNull] IEnumerable <PathFigureCollection> figures);
        void SetDirections([NotNull] IEnumerable <IDisplayNode> displayNodes);
    }
}