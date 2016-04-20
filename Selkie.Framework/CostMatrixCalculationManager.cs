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
            LineDtos = new LineDto[0];
            CostPerLine = new int[0];

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <CostMatrixResponseMessage>(subscriptionId,
                                                             CostMatrixResponseHandler);
        }

        public IRacetrackSettingsSource Settings { get; private set; }
        public IEnumerable <ILine> Lines { get; private set; }
        public IEnumerable <LineDto> LineDtos { get; private set; }
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
            ILine[] lines = m_LinesSourceManager.Lines.ToArray();
            int[] costPerLine = m_LinesSourceManager.CostPerLine.ToArray();

            if ( !IsAllConditionOkay(lines,
                                     costPerLine) )
            {
                return;
            }

            LineDto[] lineDtos = CreateLineDtos(lines);

            Lines = lines;
            CostPerLine = costPerLine;
            LineDtos = lineDtos;
            Settings = m_RacetrackSettingsSourceManager.Source;

            SendCostMatrixCalculateMessage(lineDtos,
                                           Settings);
        }

        [Status("Sending SendCostMatrixCalculateMessage...")]
        internal void SendCostMatrixCalculateMessage(
            [NotNull] LineDto[] lineDtos,
            [NotNull] IRacetrackSettingsSource settings)
        {
            var calculateMessage = new CostMatrixCalculateMessage
                                   {
                                       LineDtos = lineDtos,
                                       TurnRadiusForPort = settings.TurnRadiusForPort,
                                       TurnRadiusForStarboard = settings.TurnRadiusForStarboard,
                                       IsPortTurnAllowed = settings.IsPortTurnAllowed,
                                       IsStarboardTurnAllowed = settings.IsStarboardTurnAllowed
                                   };

            m_Bus.PublishAsync(calculateMessage);
        }


        [Status("Sending CostMatrixCalculatedMessage...")]
        internal void CostMatrixResponseHandler(CostMatrixResponseMessage message)
        {
            if ( !ValidateMatrix(message) )
            {
                return;
            }

            SendCostMatrixResponseMessage(message);
        }

        private void SendCostMatrixResponseMessage(CostMatrixResponseMessage message)
        {
            m_Converter.DoubleMatrix = message.Matrix;
            m_Converter.Convert();

            var calculatedMessage = new CostMatrixCalculatedMessage
                                    {
                                        Matrix = m_Converter.IntegerMatrix,
                                        CostPerLine = CostPerLine.ToArray()
                                    };

            m_Bus.PublishAsync(calculatedMessage);
        }

        private bool ValidateMatrix(CostMatrixResponseMessage message)
        {
            if ( message.Matrix == null ||
                 message.Matrix.Length == 0 )
            {
                HandleMatrixIsNullCase();

                return false;
            }

            if ( message.Matrix.Length != Lines.Count() * 2 )
            {
                HandleIncorrectMatrix(message);

                return false;
            }

            return true;
        }

        private void HandleIncorrectMatrix(CostMatrixResponseMessage message)
        {
            m_Logger.Info("Ignoring message! - " +
                          "Received CostMatrixResponseMessage with matrix length {0} ".Inject(message.Matrix.Length) +
                          "but expected is {0}!".Inject(Lines.Count() * 2));
        }

        [Status("Received cost matrix is null!")]
        private void HandleMatrixIsNullCase()
        {
            const string text = "Received CostMatrixResponseMessage with Matrix set to null!";
            m_Logger.Warn(text);
        }

        internal bool IsAllConditionOkay(
            [NotNull] ILine[] lines,
            [NotNull] int[] costPerLines)
        {
            if ( !lines.Any() )
            {
                m_Logger.Warn("Lines not set in LinesSourceManager!");

                return false;
            }

            if ( !costPerLines.Any() )
            {
                m_Logger.Warn("CostPerLine not set in LinesSourceManager!");

                return false;
            }

            if ( costPerLines.Length != lines.Length * 2 )
            {
                m_Logger.Warn("CostPerLine and Lines do not match in LinesSourceManager!");

                return false;
            }

            return true;
        }

        internal LineDto[] CreateLineDtos(IEnumerable <ILine> lines)
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
                                        }).ToArray();
        }
    }
}