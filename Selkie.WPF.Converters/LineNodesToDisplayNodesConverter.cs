using System;
using System.Collections.Generic;
using System.Windows.Media;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public sealed class LineNodesToDisplayNodesConverter
        : ILineNodeToDisplayLineNodeConverter,
          IDisposable
    {
        internal const double DefaultRadius = 5.0;
        internal const double DefaultStrokeThickness = 1.0;
        internal static readonly SolidColorBrush DefaultFill = Brushes.Gray;
        internal static readonly SolidColorBrush DefaultStroke = Brushes.DarkGray;
        private readonly List <IDisplayNode> m_DisplayNodes = new List <IDisplayNode>();
        private readonly IDisplayNodeFactory m_Factory;
        private IEnumerable <INodeModel> m_NodeModels = new INodeModel[0];

        public LineNodesToDisplayNodesConverter(IDisplayNodeFactory factory)
        {
            m_Factory = factory;
        }

        public void Dispose()
        {
            ReleaseDisplayNodes();
        }

        public IEnumerable <INodeModel> NodeModels
        {
            get
            {
                return m_NodeModels;
            }
            set
            {
                m_NodeModels = value;
            }
        }

        public IEnumerable <IDisplayNode> DisplayNodes
        {
            get
            {
                return m_DisplayNodes;
            }
        }

        public void Convert()
        {
            ReleaseDisplayNodes();
            LoadDisplayNodes();
        }

        internal void ReleaseDisplayNodes()
        {
            foreach ( IDisplayNode displayNode in m_DisplayNodes )
            {
                m_Factory.Release(displayNode);
            }

            m_DisplayNodes.Clear();
        }

        internal void LoadDisplayNodes()
        {
            foreach ( INodeModel nodeModel in NodeModels )
            {
                IDisplayNode displayNode = m_Factory.Create(nodeModel.Id,
                                                            nodeModel.X,
                                                            nodeModel.Y,
                                                            nodeModel.DirectionAngle.Degrees,
                                                            DefaultRadius,
                                                            DefaultStroke,
                                                            DefaultFill,
                                                            DefaultStrokeThickness);

                m_DisplayNodes.Add(displayNode);
            }
        }
    }
}