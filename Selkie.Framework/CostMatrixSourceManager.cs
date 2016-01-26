using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class CostMatrixSourceManager : ICostMatrixSourceManager
    {
        private readonly ISelkieBus m_Bus;
        private readonly ICalculateCostMatrixManager m_CalculateCostMatrixManager;
        private readonly IDoubleArrayToIntegerArrayConverter m_Converter;
        private readonly ISelkieLogger m_Logger;

        public CostMatrixSourceManager([NotNull] ISelkieLogger logger,
                                       [NotNull] ISelkieBus bus,
                                       [NotNull] ICalculateCostMatrixManager calculateCostMatrixManager,
                                       [NotNull] IDoubleArrayToIntegerArrayConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_CalculateCostMatrixManager = calculateCostMatrixManager;
            m_Converter = converter;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <CostMatrixChangedMessage>(subscriptionId,
                                                            CostMatrixChangedHandler);

            m_Bus.SubscribeAsync <ColonyLinesChangedMessage>(subscriptionId,
                                                             ColonyLinesChangedHandler);

            m_Bus.SubscribeAsync <ColonyRacetrackSettingsChangedMessage>(subscriptionId,
                                                                         ColonyRacetrackSettingsChangedHandler);

            m_CalculateCostMatrixManager.Calculate();
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
            m_CalculateCostMatrixManager.Calculate();
        }

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            m_CalculateCostMatrixManager.Calculate();
        }
    }
}