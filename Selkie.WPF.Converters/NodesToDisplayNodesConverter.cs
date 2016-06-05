using System;
using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public sealed class NodesToDisplayNodesConverter
        : INodesToDisplayNodesConverter,
          IDisposable
    {
        public NodesToDisplayNodesConverter([NotNull] IDisplayNodeFactory factory)
        {
            m_Factory = factory;
        }

        internal const double DefaultRadius = 7.0;
        internal const double DefaultStrokeThickness = 1.0;
        internal static readonly SolidColorBrush DefaultFill = Brushes.Green;
        internal static readonly SolidColorBrush DefaultStroke = Brushes.DarkGreen;
        private readonly List <IDisplayNode> m_DisplayNodes = new List <IDisplayNode>();
        private readonly IDisplayNodeFactory m_Factory;

        private IEnumerable <INodeModel> m_NodeModels = new INodeModel[]
                                                        {
                                                        };

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
            lock ( this )
            {
                ReleaseDisplayNodes();
                AddDisplayNodes();
            }
        }

        internal void AddDisplayNodes()
        {
            foreach ( INodeModel nodeModel in m_NodeModels )
            {
                IDisplayNode displayNode = CreateDisplayNode(nodeModel);

                m_DisplayNodes.Add(displayNode);
            }
        }

        internal IDisplayNode CreateDisplayNode(INodeModel nodeModel)
        {
            Angle angle = nodeModel.DirectionAngle;

            IDisplayNode displayNode = m_Factory.Create(nodeModel.Id,
                                                        nodeModel.X,
                                                        nodeModel.Y,
                                                        -angle.Degrees,
                                                        DefaultRadius,
                                                        DefaultFill,
                                                        DefaultStroke,
                                                        DefaultStrokeThickness);

            return displayNode;
        }

        internal void ReleaseDisplayNodes()
        {
            foreach ( IDisplayNode displayNode in m_DisplayNodes )
            {
                m_Factory.Release(displayNode);
            }

            m_DisplayNodes.Clear();
        }
    }
}