using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class CalculateCostMatrixManager : ICalculateCostMatrixManager
    {
        private readonly ISelkieBus m_Bus;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ISelkieLogger m_Logger;
        private readonly IRacetrackSettingsSourceManager m_RacetrackSettingsSourceManager;

        public CalculateCostMatrixManager([NotNull] ISelkieLogger logger,
                                          [NotNull] ISelkieBus bus,
                                          [NotNull] ILinesSourceManager linesSourceManager,
                                          [NotNull] IRacetrackSettingsSourceManager racetrackSettingsSourceManager)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;
            m_RacetrackSettingsSourceManager = racetrackSettingsSourceManager;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <ColonyLinesChangedMessage>(subscriptionId,
                                                             ColonyLinesChangedHandler);

            m_Bus.SubscribeAsync <ColonyRacetrackSettingsChangedMessage>(subscriptionId,
                                                                         ColonyRacetrackSettingsChangedHandler);
        }

        public void Calculate()
        {
            SendRacetrackSettingsSetMessage();
            SendLinesSetMessage();
        }

        private void SendRacetrackSettingsSetMessage()
        {
            IRacetrackSettingsSource source = m_RacetrackSettingsSourceManager.Source;

            var message = new RacetrackSettingsSetMessage
                          {
                              TurnRadiusForPort = source.TurnRadiusForPort,
                              TurnRadiusForStarboard = source.TurnRadiusForStarboard,
                              IsPortTurnAllowed = source.IsPortTurnAllowed,
                              IsStarboardTurnAllowed = source.IsStarboardTurnAllowed
                          };

            m_Bus.PublishAsync(message);
        }

        private void SendLinesSetMessage()
        {
            LineDto[] dtos = CreateLineDtos(m_LinesSourceManager.Lines).ToArray();

            var linesSetMessage = new LinesSetMessage
                                  {
                                      LineDtos = dtos
                                  };

            m_Bus.PublishAsync(linesSetMessage);

            m_Logger.Debug("Sent LinesSetMessage with {0} lines in it!",
                           dtos.Length);
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

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            m_Bus.PublishAsync(new CostMatrixCalculateMessage());
        }

        internal void ColonyRacetrackSettingsChangedHandler([NotNull] ColonyRacetrackSettingsChangedMessage message)
        {
            m_Bus.PublishAsync(new CostMatrixCalculateMessage());
        }
    }
}