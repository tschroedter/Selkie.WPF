using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Common;
using Selkie.Common.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;

// todo remove LineDirection from Selkie.Common.Constants

namespace Selkie.WPF.Converters
{
    public sealed class PathToLineToLineNodeConverter
        : IPathToLineToLineNodeConverter,
          IDisposable
    {
        public PathToLineToLineNodeConverter([NotNull] IDisposer disposer,
                                             [NotNull] IConverterFactory converterFactory,
                                             [NotNull] INodeIndexHelper nodeIndexHelper)
        {
            m_Disposer = disposer;
            m_ConverterFactory = converterFactory;
            m_NodeIndexHelper = nodeIndexHelper;

            m_Disposer.AddResource(ReleaseConverters);
        }

        private readonly IConverterFactory m_ConverterFactory;
        private readonly IDisposer m_Disposer;
        private readonly INodeIndexHelper m_NodeIndexHelper;
        private List <ILineToLineNodeConverter> m_Nodes = new List <ILineToLineNodeConverter>();

        private IEnumerable <int> m_Path = new int[]
                                           {
                                           };

        public void Dispose()
        {
            m_Disposer.Dispose();
        }

        public void Convert()
        {
            int[] pathArray = m_Path.ToArray();

            var nodes = new List <ILineToLineNodeConverter>();

            for ( var i = 0 ; i < pathArray.Length - 1 ; i++ )
            {
                int fromIndex = pathArray [ i ];
                int toIndex = pathArray [ i + 1 ];

                ILineToLineNodeConverter node = CreateLineToLineNode(fromIndex,
                                                                     toIndex);

                nodes.Add(node);
            }

            m_Nodes = nodes;
        }

        public IEnumerable <int> Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        public IEnumerable <ILineToLineNodeConverter> Nodes
        {
            get
            {
                return m_Nodes;
            }
        }

        internal ILineToLineNodeConverter CreateLineToLineNode(int fromIndex,
                                                               int toIndex)
        {
            ILine fromLine = m_NodeIndexHelper.NodeIndexToLine(fromIndex);
            ILine toLine = m_NodeIndexHelper.NodeIndexToLine(toIndex);

            Constants.LineDirection fromDirection = m_NodeIndexHelper.NodeIndexToDirection(fromIndex);
            Constants.LineDirection toDirection = m_NodeIndexHelper.NodeIndexToDirection(toIndex);


            var node = m_ConverterFactory.Create <ILineToLineNodeConverter>();

            node.From = fromLine;
            node.FromDirection = fromDirection;
            node.To = toLine;
            node.ToDirection = toDirection;

            node.Convert();

            return node;
        }

        internal void ReleaseConverters()
        {
            foreach ( ILineToLineNodeConverter converter in m_Nodes )
            {
                m_ConverterFactory.Release(converter);
            }

            m_Nodes.Clear();
        }
    }
}