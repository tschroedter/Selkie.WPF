using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converter;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Services.Racetracks.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class CostMatrixSourceManager : ICostMatrixSourceManager
    {
        private readonly IBus m_Bus;
        private readonly IDoubleArrayToIntegerArrayConverter m_Converter;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ILogger m_Logger;
        private readonly IRacetrackSettingsSourceManager m_RacetrackSettingsSourceManager;

        public CostMatrixSourceManager([NotNull] ILogger logger,
                                       [NotNull] IBus bus,
                                       [NotNull] ILinesSourceManager linesSourceManager,
                                       [NotNull] IRacetrackSettingsSourceManager racetrackSettingsSourceManager,
                                       [NotNull] IDoubleArrayToIntegerArrayConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;
            m_RacetrackSettingsSourceManager = racetrackSettingsSourceManager;
            m_Converter = converter;

            string subscriptionId = GetType().FullName;

            bus.SubscribeHandlerAsync <CostMatrixChangedMessage>(logger,
                                                                 subscriptionId,
                                                                 CostMatrixChangedHandler);

            m_Bus.SubscribeHandlerAsync <ColonyLinesChangedMessage>(logger,
                                                                    subscriptionId,
                                                                    ColonyLinesChangedHandler);

            m_Bus.SubscribeHandlerAsync <ColonyRacetrackSettingsChangedMessage>(logger,
                                                                                subscriptionId,
                                                                                ColonyRacetrackSettingsChangedHandler);
            bus.PublishAsync(new CostMatrixGetMessage()); // todo rename to ...RequestMessage
        }

        public int[][] Matrix
        {
            get
            {
                return m_Converter.IntegerMatrix;
            }
        }

        internal void CostMatrixChangedHandler(CostMatrixChangedMessage message)
        {
            if ( message.Matrix == null )
            {
                m_Logger.Warn("Received CostMatrixChangedMessage with Matrix set to null!");
                return;
            }

            m_Converter.DoubleMatrix = message.Matrix;
            m_Converter.Convert();

            m_Bus.PublishAsync(new ColonyCostMatrixChangedMessage());
        }

        internal void ColonyRacetrackSettingsChangedHandler(ColonyRacetrackSettingsChangedMessage message)
        {
            RecalculateCostMatrix();
        }

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            RecalculateCostMatrix();
        }

        internal void RecalculateCostMatrix()
        {
            SendRacetrackSettingsSetMessage();
            SendLinesSetMessage();
            SendCostMatrixCalculateMessage();
        }

        internal void SendRacetrackSettingsSetMessage()
        {
            IRacetrackSettingsSource source = m_RacetrackSettingsSourceManager.Source;
            var message = new RacetrackSettingsSetMessage
                          {
                              TurnRadiusInMetres = source.TurnRadius,
                              IsPortTurnAllowed = source.IsPortTurnAllowed,
                              IsStarboardTurnAllowed = source.IsStarboardTurnAllowed
                          };

            m_Bus.PublishAsync(message);
        }

        internal void SendLinesSetMessage()
        {
            IEnumerable <LineDto> dtos = CreateLineDtos(m_LinesSourceManager.Lines);
            var linesSetMessage = new LinesSetMessage
                                  {
                                      LineDtos = dtos.ToArray()
                                  };

            m_Bus.PublishAsync(linesSetMessage);
        }

        internal IEnumerable <LineDto> CreateLineDtos(IEnumerable <ILine> lines)
        {
            return lines.Select(line => new LineDto
                                        {
                                            RunDirection = line.RunDirection.ToString(),
                                            Id = line.Id,
                                            IsUnknown = line.IsUnknown,
                                            X1 = line.X1,
                                            Y1 = line.Y1,
                                            X2 = line.X2,
                                            Y2 = line.Y2
                                        }).ToList();
        }

        internal void SendCostMatrixCalculateMessage()
        {
            m_Bus.PublishAsync(new CostMatrixCalculateMessage());
        }
    }
}