using System.Linq;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
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
        private readonly IAcoProxyLogger m_AcoProxylogger;
        private readonly IBus m_Bus;
        private readonly ICostMatrixSourceManager m_CostMatrixSourceManager;
        private readonly ILinesSourceManager m_LinesSourceManager;

        public ServiceProxy([NotNull] ILogger alogger,
                            [NotNull] IAcoProxyLogger acoProxylogger,
                            [NotNull] IBus bus,
                            [NotNull] ICostMatrixSourceManager costMatrixSourceManager,
                            [NotNull] ILinesSourceManager linesSourceManager)
        {
            m_AcoProxylogger = acoProxylogger;
            m_Bus = bus;
            m_CostMatrixSourceManager = costMatrixSourceManager;
            m_LinesSourceManager = linesSourceManager;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeHandlerAsync <CreatedColonyMessage>(alogger,
                                                               subscriptionId,
                                                               CreatedColonyHandler);

            m_Bus.SubscribeHandlerAsync <StartedMessage>(alogger,
                                                         subscriptionId,
                                                         StartedHandler);

            m_Bus.SubscribeHandlerAsync <StoppedMessage>(alogger,
                                                         subscriptionId,
                                                         StoppedHandler);

            m_Bus.SubscribeHandlerAsync <FinishedMessage>(alogger,
                                                          subscriptionId,
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
                                    CostPerLine = m_LinesSourceManager.CostPerLine.ToArray() // todo .ToArray() not nice
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
                                   Times = 2000 // todo configure this value
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