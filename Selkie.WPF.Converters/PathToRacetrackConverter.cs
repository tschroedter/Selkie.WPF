using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class PathToRacetracksConverter : IPathToRacetracksConverter
    {
        private readonly ISelkieLogger m_Logger;
        private readonly INodeIdHelper m_NodeIdHelper;
        private readonly List <IPath> m_Paths = new List <IPath>();
        private readonly IRacetracksSourceManager m_RacetracksSourceManager;
        private int[] m_Path;

        public PathToRacetracksConverter([NotNull] ISelkieLogger logger,
                                         [NotNull] ISelkieBus bus,
                                         [NotNull] INodeIdHelper nodeIdHelper,
                                         [NotNull] IRacetracksSourceManager racetracksSourceManager)
        {
            m_Logger = logger;
            m_NodeIdHelper = nodeIdHelper;
            m_RacetracksSourceManager = racetracksSourceManager;

            Update();

            bus.SubscribeAsync <ColonyRacetracksResponseMessage>(GetType().FullName,
                                                                 ColonyRacetracksResponseHandler);
        }

        internal IRacetracks Racetracks { get; private set; }

        internal void Update()
        {
            Racetracks = m_RacetracksSourceManager.Racetracks;
        }

        internal void ColonyRacetracksResponseHandler(ColonyRacetracksResponseMessage message)
        {
            Update();
        }

        internal IPath GetRacetrack(int[] path,
                                    int currentNode,
                                    int nextNode)
        {
            int fromLineId = m_NodeIdHelper.NodeToLine(currentNode);
            int toLineId = m_NodeIdHelper.NodeToLine(nextNode);

            if ( fromLineId < 0 ||
                 fromLineId >= path.Length ||
                 toLineId < 0 ||
                 toLineId >= path.Length )
            {
                LogErrorIndexOutOfBounds(path,
                                         currentNode,
                                         nextNode,
                                         fromLineId,
                                         toLineId);

                return Framework.Common.Path.Unknown;
            }

            bool isFromNodeForward = m_NodeIdHelper.IsForwardNode(currentNode);
            bool isToNodeForward = m_NodeIdHelper.IsForwardNode(nextNode);

            if ( isFromNodeForward )
            {
                return GetPathForNodeForward(isToNodeForward,
                                             fromLineId,
                                             toLineId);
            }

            return GetPathToNodeFoward(isToNodeForward,
                                       fromLineId,
                                       toLineId);
        }

        private void LogErrorIndexOutOfBounds(int[] path,
                                              int currentNode,
                                              int nextNode,
                                              int fromLineId,
                                              int toLineId)
        {
            string message =
                "Index out of bounds: fromLineId: {0} toLineId: {1} Length: {2} currentNode: {3} nextNode: {4}"
                    .Inject(
                            fromLineId,
                            toLineId,
                            path.Length,
                            currentNode,
                            nextNode);

            m_Logger.Error(message);
        }

        internal IPath GetPathToNodeFoward(bool isToNodeForward,
                                           int fromLineId,
                                           int toLineId)
        {
            if ( Racetracks.ReverseToForward.Length > fromLineId &&
                 Racetracks.ReverseToForward [ 0 ].Length > toLineId )
            {
                return isToNodeForward
                           ? Racetracks.ReverseToForward [ fromLineId ] [ toLineId ]
                           : Racetracks.ReverseToReverse [ fromLineId ] [ toLineId ];
            }

            m_Logger.Warn("From Line Id {0} and/or To Line Id {1} are invalid!".Inject(fromLineId,
                                                                                       toLineId));

            return Framework.Common.Path.Unknown;
        }

        internal IPath GetPathForNodeForward(bool isToNodeForward,
                                             int fromLineId,
                                             int toLineId)
        {
            if ( Racetracks.ForwardToForward.Length > fromLineId &&
                 Racetracks.ForwardToForward [ 0 ].Length > toLineId )
            {
                return isToNodeForward
                           ? Racetracks.ForwardToForward [ fromLineId ] [ toLineId ]
                           : Racetracks.ForwardToReverse [ fromLineId ] [ toLineId ];
            }

            m_Logger.Warn("From Line Id {0} and/or To Line Id {1} are invalid!".Inject(fromLineId,
                                                                                       toLineId));

            return Framework.Common.Path.Unknown;
        }

        #region IPathToRacetracksConverter Members

        public IEnumerable <int> Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value.ToArray();
            }
        }

        public IEnumerable <IPath> Paths
        {
            get
            {
                return m_Paths;
            }
        }

        public void Convert()
        {
            m_Paths.Clear();

            string pathString = string.Join(",",
                                            m_Path);

            for ( var i = 0 ; i < m_Path.Length - 1 ; i++ )
            {
                int currentNode = m_Path [ i ];
                int nextNode = m_Path [ i + 1 ];

                IPath racetrack = GetRacetrack(m_Path,
                                               currentNode,
                                               nextNode);

                m_Logger.Debug("[Look] Convert path: {0} Line: {1} To: {2} -=> Racetrack Length: {3}".Inject(pathString,
                                                                                                             currentNode,
                                                                                                             nextNode,
                                                                                                             racetrack
                                                                                                                 .Distance));

                m_Paths.Add(racetrack);
            }
        }

        #endregion
    }
}