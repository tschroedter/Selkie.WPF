using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Aco
{
    [Interceptor(typeof ( StatusAspect ))]
    [ProjectComponent(Lifestyle.Transient)]
    public class ServiceProxy : IServiceProxy
    {
        internal const int DefaultNumberOfIterations = 2000;
        private readonly IAcoProxyLogger m_AcoProxylogger;
        private readonly ISelkieBus m_Bus;
        private readonly ISelkieInMemoryBus m_MemoryBus;
        private readonly IColonyParametersValidator m_Validator;

        public ServiceProxy([NotNull] IAcoProxyLogger acoProxylogger,
                            [NotNull] ISelkieBus bus,
                            [NotNull] ISelkieInMemoryBus memoryBus,
                            [NotNull] IColonyParametersValidator validator)
        {
            m_AcoProxylogger = acoProxylogger;
            m_Bus = bus;
            m_MemoryBus = memoryBus;
            m_Validator = validator;

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

        [Status("Received request to create colony.")]
        public void CreateColony(IColonyParameters colonyParameters)
        {
            IsColonyCreated = false;
            IsRunning = false;

            m_AcoProxylogger.LogCostMatrix(colonyParameters.CostMatrix);
            m_AcoProxylogger.LogCostPerLine(colonyParameters.CostPerLine);

            m_Validator.Validate(colonyParameters);

            var createMessage = new CreateColonyMessage
                                {
                                    CostMatrix = colonyParameters.CostMatrix,
                                    CostPerLine = colonyParameters.CostPerLine,
                                    IsFixedStartNode = colonyParameters.IsFixedStartNode,
                                    FixedStartNode = colonyParameters.FixedStartNode
                                };

            m_Bus.PublishAsync(createMessage);
        }

        [Status("Received request to start colony calculation.")]
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

        [Status("Received request to stop colony calculation.")]
        public void Stop()
        {
            m_Bus.PublishAsync(new StopRequestMessage());
        }

        public bool IsFinished { get; private set; }

        [Status("Colony was created!")]
        internal void CreatedColonyHandler(CreatedColonyMessage message)
        {
            IsColonyCreated = true;
            IsFinished = false;
        }

        [Status("Colony calculation started...")]
        internal void StartedHandler(StartedMessage message)
        {
            IsRunning = true;
            IsFinished = false;

            m_MemoryBus.PublishAsync(new ColonyStartedMessage());
        }

        [Status("Colony calculation was stopped!")]
        internal void StoppedHandler(StoppedMessage message)
        {
            IsRunning = false;
            IsColonyCreated = false;
            IsFinished = false;

            m_MemoryBus.PublishAsync(new ColonyStoppedMessage());
        }

        [Status("Colony calculation finished!")]
        internal void FinishedHandler(FinishedMessage message)
        {
            IsRunning = false;
            IsColonyCreated = false;
            IsFinished = true;

            m_MemoryBus.PublishAsync(new ColonyFinishedMessage());
        }
    }
}