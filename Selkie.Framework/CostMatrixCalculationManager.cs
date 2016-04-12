using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Messages;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework
{
    [Interceptor(typeof ( StatusAspect ))]
    [ProjectComponent(Lifestyle.Transient)]
    public class CostMatrixCalculationManager : ICostMatrixCalculationManager
    {
        // todo testing (Check if there is a SAGA in EasyNetQ)
        private readonly ISelkieBus m_Bus;
        private readonly IDoubleArrayToIntegerArrayConverter m_Converter;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ISelkieLogger m_Logger;
        private readonly IRacetrackSettingsSourceManager m_RacetrackSettingsSourceManager;

        public CostMatrixCalculationManager([NotNull] ISelkieLogger logger,
                                            [NotNull] ISelkieBus bus,
                                            [NotNull] ILinesSourceManager linesSourceManager,
                                            [NotNull] IRacetrackSettingsSourceManager racetrackSettingsSourceManager,
                                            [NotNull] IDoubleArrayToIntegerArrayConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;
            m_RacetrackSettingsSourceManager = racetrackSettingsSourceManager;
            m_Converter = converter;

            Lines = new ILine[0];
            CostPerLine = new int[0];

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <LinesChangedMessage>(subscriptionId,
                                                       LinesChangedHandler);

            m_Bus.SubscribeAsync <RacetrackSettingsChangedMessage>(subscriptionId,
                                                                   RacetrackSettingsChangedHandler);

            m_Bus.SubscribeAsync <CostMatrixChangedMessage>(subscriptionId,
                                                            CostMatrixChangedHandler);
        }

        public bool IsCalculating { get; private set; }
        public bool IsReceivedColonyLinesChangedMessage { get; private set; }
        public bool IsReceivedColonyRacetrackSettingsChangedMessage { get; private set; }
        public IEnumerable <ILine> Lines { get; private set; }
        public IEnumerable <int> CostPerLine { get; private set; }

        public int[][] Matrix
        {
            get
            {
                return m_Converter.IntegerMatrix;
            }
        }

        [Status("Calculating cost matrix...")]
        public void Calculate()
        {
            if ( IsCalculating )
            {
                return;
            }

            IsCalculating = true;
            IsReceivedColonyLinesChangedMessage = false;
            IsReceivedColonyRacetrackSettingsChangedMessage = false;

            Lines = m_LinesSourceManager.Lines;
            CostPerLine = m_LinesSourceManager.CostPerLine;

            SendLinesSetMessage();
        }

        [Status("Sending LinesSetMessage...")]
        private void SendLinesSetMessage()
        {
            LineDto[] dtos = CreateLineDtos(Lines).ToArray();

            var linesSetMessage = new LinesSetMessage
                                  {
                                      LineDtos = dtos
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

        [Status("Received LinesChangedMessage...")]
        internal void LinesChangedHandler(LinesChangedMessage message)
        {
            if ( !IsCalculating )
            {
                return;
            }

            if ( Lines.Count() != message.LineDtos.Length )
            {
                m_Logger.Info("Ignoring message! - " +
                              "Received LinesChangedMessage with {0} ".Inject(message.LineDtos.Length) +
                              "lines put expected {0}!".Inject(Lines.Count()));
                return;
            }

            IsReceivedColonyLinesChangedMessage = true;

            SendRacetrackSettingsSetMessage();
        }

        [Status("Sending RacetrackSettingsSetMessage...")]
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


        [Status("Received RacetrackSettingsChangedMessage...")]
        internal void RacetrackSettingsChangedHandler([NotNull] RacetrackSettingsChangedMessage message)
        {
            if ( !IsCalculating )
            {
                return;
            }

            IsReceivedColonyRacetrackSettingsChangedMessage = true;

            m_Bus.PublishAsync(new CostMatrixCalculateMessage());
        }

        [Status("Sending CostMatrixCalculatedMessage...")]
        internal void CostMatrixChangedHandler(CostMatrixChangedMessage message)
        {
            if ( !IsCalculating )
            {
                return;
            }

            IsCalculating = false;

            if ( message.Matrix == null )
            {
                HandleMatrixIsNullCase();

                return;
            }

            m_Converter.DoubleMatrix = message.Matrix;
            m_Converter.Convert();

            m_Bus.PublishAsync(new CostMatrixCalculatedMessage
                               {
                                   Matrix = m_Converter.IntegerMatrix,
                                   CostPerLine = CostPerLine.ToArray()
                               });
        }

        [Status("Received cost matrix is null!")]
        private void HandleMatrixIsNullCase()
        {
            const string text = "Received CostMatrixChangedMessage with Matrix set to null!";
            m_Logger.Warn(text);
        }
    }
}