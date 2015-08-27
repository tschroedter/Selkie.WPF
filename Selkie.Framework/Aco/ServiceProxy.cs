using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Aco
{
    [ProjectComponent(Lifestyle.Transient)]
    public class ServiceProxy : IServiceProxy
    {
        internal const int DefaultNumberOfIterations = 2000;
        private readonly IAcoProxyLogger m_AcoProxylogger;
        private readonly ISelkieBus m_Bus;
        private readonly ICostMatrixSourceManager m_CostMatrixSourceManager;
        private readonly ILinesSourceManager m_LinesSourceManager;

        public ServiceProxy([NotNull] IAcoProxyLogger acoProxylogger,
                            [NotNull] ISelkieBus bus,
                            [NotNull] ICostMatrixSourceManager costMatrixSourceManager,
                            [NotNull] ILinesSourceManager linesSourceManager)
        {
            m_AcoProxylogger = acoProxylogger;
            m_Bus = bus;
            m_CostMatrixSourceManager = costMatrixSourceManager;
            m_LinesSourceManager = linesSourceManager;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <CreatedColonyMessage>(subscriptionId,
                                                        CreatedColonyHandler);

            m_Bus.SubscribeAsync <StartedMessage>(subscriptionId,
                                                  StartedHandler);

            m_Bus.SubscribeAsync <StoppedMessage>(subscriptionId,
                                                  StoppedHandler);

            m_Bus.SubscribeAsync <FinishedMessage>(subscriptionId,
                                                   FinishedHandler);
        }

        public bool IsColonyCreated { get; private set; }
        public bool IsRunning { get; private set; }

        public void CreateColony()
        {
            IsColonyCreated = false;
            IsRunning = false;

            m_AcoProxylogger.LogCostMatrix(m_CostMatrixSourceManager.Matrix);
            m_AcoProxylogger.LogCostPerLine(m_LinesSourceManager.CostPerLine.ToArray());

            if ( m_CostMatrixSourceManager.Matrix.Length == 0 )
            {
                m_AcoProxylogger.Error("Cost Matrix is not set!");

                return;
            }

            var createMessage = new CreateColonyMessage
                                {
                                    CostMatrix = m_CostMatrixSourceManager.Matrix,
                                    CostPerLine = m_LinesSourceManager.CostPerLine.ToArray()
                                };

            m_Bus.PublishAsync(createMessage);
        }

        public bool Start()
        {
            if ( !IsColonyCreated )
            {
                m_AcoProxylogger.Error("Colony doesn't exists!");

                return false;
            }

            m_Bus.PublishAsync(new StartMessage
                               {
                                   Times = DefaultNumberOfIterations
                               });

            return true;
        }

        public void Stop()
        {
            m_Bus.PublishAsync(new StopRequestMessage());
        }

        public bool IsFinished { get; private set; }

        internal void BestTrailHandler(BestTrailMessage message)
        {
            m_Bus.PublishAsync(new ColonyBestTrailMessage
                               {
                                   Alpha = message.Alpha,
                                   Beta = message.Beta,
                                   Gamma = message.Gamma,
                                   Iteration = message.Iteration,
                                   Trail = message.Trail,
                                   Type = message.Type,
                                   Length = message.Length
                               });
        }

        internal void CreatedColonyHandler(CreatedColonyMessage message)
        {
            IsColonyCreated = true;
            IsFinished = false;
        }

        internal void StartedHandler(StartedMessage message)
        {
            IsRunning = true;
            IsFinished = false;

            m_Bus.PublishAsync(new ColonyStartedMessage());
        }

        internal void StoppedHandler(StoppedMessage message)
        {
            IsRunning = false;
            IsColonyCreated = false;
            IsFinished = false;

            m_Bus.PublishAsync(new ColonyStoppedMessage());
        }

        internal void FinishedHandler(FinishedMessage message)
        {
            IsRunning = false;
            IsColonyCreated = false;
            IsFinished = true;

            m_Bus.PublishAsync(new ColonyFinishedMessage());
        }
    }
}