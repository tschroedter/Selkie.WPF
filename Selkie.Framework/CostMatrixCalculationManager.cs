using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Framework.Messages;
using Selkie.Geometry.Surveying;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework
{
    [Interceptor(typeof( StatusAspect ))]
    [ProjectComponent(Lifestyle.Transient)]
    public class CostMatrixCalculationManager : ICostMatrixCalculationManager
    {
        public CostMatrixCalculationManager([NotNull] ISelkieLogger logger,
                                            [NotNull] ISelkieBus bus,
                                            [NotNull] ILinesSourceManager linesSourceManager,
                                            [NotNull] IRacetrackSettingsSourceManager racetrackSettingsSourceManager,
                                            [NotNull] IDoubleArrayToIntegerArrayConverter converter,
                                            [NotNull] ISurveyFeatureToSurveyFeatureDtoConverter dtoConverter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;
            m_RacetrackSettingsSourceManager = racetrackSettingsSourceManager;
            m_Converter = converter;
            m_DtoConverter = dtoConverter;

            CostPerFeature = new int[0];
            SurveyFeature = new ISurveyFeature[0];

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <CostMatrixResponseMessage>(subscriptionId,
                                                             CostMatrixResponseHandler);
        }

        public IRacetrackSettingsSource Settings { get; private set; }
        private readonly ISelkieBus m_Bus;
        private readonly IDoubleArrayToIntegerArrayConverter m_Converter;
        private readonly ISurveyFeatureToSurveyFeatureDtoConverter m_DtoConverter;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ISelkieLogger m_Logger;
        private readonly IRacetrackSettingsSourceManager m_RacetrackSettingsSourceManager;
        public IEnumerable <int> CostPerFeature { get; private set; }
        public ISurveyFeature[] SurveyFeature { get; private set; }

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
            ISurveyFeature[] features = m_LinesSourceManager.SurveyFeatures.ToArray();
            int[] costPerLine = m_LinesSourceManager.CostPerFeature.ToArray();

            if ( !IsAllConditionOkay(features,
                                     costPerLine) )
            {
                return;
            }

            SurveyFeatureDto[] dtos = CreateDtos(features);

            SurveyFeature = features;
            CostPerFeature = costPerLine;
            Settings = m_RacetrackSettingsSourceManager.Source;

            SendCostMatrixCalculateMessage(dtos,
                                           Settings);
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

        internal SurveyFeatureDto[] CreateDtos(IEnumerable <ISurveyFeature> features)
        {
            m_DtoConverter.Features = features;
            m_DtoConverter.Convert();

            return m_DtoConverter.Dtos.ToArray();
        }

        internal bool IsAllConditionOkay(
            [NotNull] ISurveyFeature[] features,
            [NotNull] int[] costPerFeature)
        {
            if ( !features.Any() )
            {
                m_Logger.Warn("SurveyFeature not set in LinesSourceManager!");

                return false;
            }

            if ( !costPerFeature.Any() )
            {
                m_Logger.Warn("CostPerFeature not set in LinesSourceManager!");

                return false;
            }

            // ReSharper disable once InvertIf
            if ( costPerFeature.Length != features.Length * 2 )
            {
                m_Logger.Warn("CostPerFeature and Lines do not match in LinesSourceManager!");

                return false;
            }

            return true;
        }

        [Status("Sending SendCostMatrixCalculateMessage...")]
        internal void SendCostMatrixCalculateMessage(
            [NotNull] SurveyFeatureDto[] dtos,
            [NotNull] IRacetrackSettingsSource settings)
        {
            var calculateMessage = new CostMatrixCalculateMessage
                                   {
                                       SurveyFeatureDtos = dtos,
                                       TurnRadiusForPort = settings.TurnRadiusForPort,
                                       TurnRadiusForStarboard = settings.TurnRadiusForStarboard,
                                       IsPortTurnAllowed = settings.IsPortTurnAllowed,
                                       IsStarboardTurnAllowed = settings.IsStarboardTurnAllowed
                                   };

            m_Bus.PublishAsync(calculateMessage);
        }

        private void HandleIncorrectMatrix(CostMatrixResponseMessage message)
        {
            m_Logger.Info("Ignoring message! - " +
                          "Received CostMatrixResponseMessage with matrix length {0} ".Inject(message.Matrix.Length) +
                          "but expected is {0}!".Inject(SurveyFeature.Length * 2));
        }

        [Status("Received cost matrix is null!")]
        private void HandleMatrixIsNullCase()
        {
            const string text = "Received CostMatrixResponseMessage with Matrix set to null!";
            m_Logger.Warn(text);
        }

        private void SendCostMatrixResponseMessage(CostMatrixResponseMessage message)
        {
            m_Converter.DoubleMatrix = message.Matrix;
            m_Converter.Convert();

            var calculatedMessage = new CostMatrixCalculatedMessage
                                    {
                                        Matrix = m_Converter.IntegerMatrix,
                                        CostPerFeature = CostPerFeature.ToArray()
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

            if ( message.Matrix.Length == SurveyFeature.Length * 2 )
            {
                return true;
            }

            HandleIncorrectMatrix(message);

            return false;
        }
    }
}