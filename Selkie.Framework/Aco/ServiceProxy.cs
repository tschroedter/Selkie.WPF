using System;
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

            int[][] costMatrix = m_CostMatrixSourceManager.Matrix;
            int[] costPerLine = m_LinesSourceManager.CostPerLine.ToArray();

            m_AcoProxylogger.LogCostMatrix(costMatrix);
            m_AcoProxylogger.LogCostPerLine(costPerLine);

            ValidateCostMatrixAndCostPerLine(costMatrix,
                                             costPerLine);

            var createMessage = new CreateColonyMessage
                                {
                                    CostMatrix = costMatrix,
                                    CostPerLine = costPerLine
                                };

            m_Bus.PublishAsync(createMessage);
        }

        // ReSharper disable UnusedParameter.Local
        private void ValidateCostMatrixAndCostPerLine(int[][] costMatrix,
                                                      int[] costPerLine)
        // ReSharper restore UnusedParameter.Local
        {
            if ( costMatrix.Length == 0 )
            {
                throw new ArgumentException("Cost Matrix is not set!");
            }

            if ( costPerLine.Length == 0 )
            {
                throw new ArgumentException("CostPerLine array is not set!");
            }

            if ( costPerLine.Length != costMatrix.Length )
            {
                throw new ArgumentException("CostMatrix and CostPerLine do not match!");
            }
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