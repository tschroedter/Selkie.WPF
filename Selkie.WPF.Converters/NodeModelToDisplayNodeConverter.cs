using System;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public sealed class NodeModelToDisplayNodeConverter
        : INodeModelToDisplayNodeConverter,
          IDisposable
    {
        internal const double DefaultRadius = 7.0;
        internal const double DefaultStrokeThickness = 1.0;
        internal static readonly SolidColorBrush DefaultFill = Brushes.Green;
        internal static readonly SolidColorBrush DefaultStroke = Brushes.DarkGreen;
        private readonly IDisposer m_Disposer;
        private readonly IDisplayNodeFactory m_Factory;
        private IDisplayNode m_DisplayNode = Common.DisplayNode.Unknown;
        private SolidColorBrush m_FillBrush = DefaultFill;
        private SolidColorBrush m_StrokeBrush = DefaultStroke;

        public NodeModelToDisplayNodeConverter([NotNull] IDisposer disposer,
                                               [NotNull] IDisplayNodeFactory factory)
        {
            m_Disposer = disposer;
            m_Factory = factory;
        }

        public void Dispose()
        {
            m_Disposer.Dispose();
        }

        public SolidColorBrush FillBrush
        {
            get
            {
                return m_FillBrush;
            }
            set
            {
                m_FillBrush = value;
            }
        }

        public SolidColorBrush StrokeBrush
        {
            get
            {
                return m_StrokeBrush;
            }
            set
            {
                m_StrokeBrush = value;
            }
        }

        public INodeModel NodeModel { get; set; }

        public IDisplayNode DisplayNode
        {
            get
            {
                return m_DisplayNode;
            }
        }

        public void Convert()
        {
            ReleaseDisplayNode();

            m_DisplayNode = m_Factory.Create(NodeModel.Id,
                                             NodeModel.X,
                                             NodeModel.Y,
                                             NodeModel.DirectionAngle.Degrees,
                                             DefaultRadius,
                                             m_FillBrush,
                                             m_StrokeBrush,
                                             DefaultStrokeThickness);
        }

        public void ReleaseDisplayNode()
        {
            if ( m_DisplayNode.Id != Common.DisplayNode.UnknownId )
            {
                m_Factory.Release(m_DisplayNode);
            }
        }
    }
}