using System;
using System.Threading;
using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Framework.Messages;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [Interceptor(typeof ( StatusAspect ))]
    [Interceptor(typeof ( MessageHandlerAspect ))]
    [ProjectComponent(Lifestyle.Singleton)]
    public sealed class Colony : IColony
    {
        internal const int SleepTimeOneSecond = 1000;
        private readonly IAntSettingsSourceManager m_AntSettingsSourceManager;
        private readonly ISelkieBus m_Bus;
        private readonly IColonyParametersFactory m_ColonyParametersFactory;
        private readonly ISelkieLogger m_Logger;
        private readonly ICostMatrixCalculationManager m_Manager;
        private readonly IServiceProxy m_ServiceProxy;

        public Colony([NotNull] ISelkieLogger logger,
                      [NotNull] ISelkieBus bus,
                      [NotNull] ISelkieInMemoryBus memoryBus,
                      [NotNull] IServiceProxy serviceProxy,
                      [NotNull] ICostMatrixCalculationManager manager,
                      [NotNull] IAntSettingsSourceManager antSettingsSourceManager,
                      [NotNull] IColonyParametersFactory colonyParametersFactory)

        {
            SleepTimeInMs = SleepTimeOneSecond;
            m_Logger = logger;
            m_Bus = bus;
            ISelkieInMemoryBus memoryBus1 = memoryBus;
            m_ServiceProxy = serviceProxy;
            m_Manager = manager;
            m_AntSettingsSourceManager = antSettingsSourceManager;
            m_ColonyParametersFactory = colonyParametersFactory;

            string subscriptionId = GetType().FullName;

            memoryBus1.SubscribeAsync <ColonyStartRequestMessage>(subscriptionId,
                                                                  ColonyStartRequestHandler);

            memoryBus1.SubscribeAsync <ColonyStopRequestMessage>(subscriptionId,
                                                                 ColonyStopRequestHandler);

            memoryBus1.SubscribeAsync <ColonyPheromonesRequestMessage>(subscriptionId,
                                                                       ColonyPheromonesRequestHandler);
            m_Bus.SubscribeAsync <CostMatrixCalculatedMessage>(subscriptionId,
                                                               CostMatrixCalculatedMessageHandler);
        }

        public int SleepTimeInMs { get; private set; }

        public bool IsRunning
        {
            get
            {
                return m_ServiceProxy.IsRunning;
            }
        }

        public void SetSleepTimeInMs(int value)
        {
            if ( value > 0 )
            {
                SleepTimeInMs = value;
            }
        }

        internal void ColonyStartRequestHandler(ColonyStartRequestMessage message)
        {
            m_Manager.Calculate();
        }

        internal void ColonyStopRequestHandler(ColonyStopRequestMessage message)
        {
            m_ServiceProxy.Stop();
        }

        internal void ColonyPheromonesRequestHandler(ColonyPheromonesRequestMessage message)
        {
            m_Bus.PublishAsync(new PheromonesRequestMessage());
        }

        [Status("Waiting for colony to be created...")]
        internal void CostMatrixCalculatedMessageHandler(CostMatrixCalculatedMessage message)
        {
            IAntSettingsSource antSettingsSource = m_AntSettingsSourceManager.Source;

            IColonyParameters colonyParameters = m_ColonyParametersFactory.Create(message.Matrix,
                                                                                  message.CostPerLine,
                                                                                  antSettingsSource.IsFixedStartNode,
                                                                                  antSettingsSource.FixedStartNode);

            m_ServiceProxy.CreateColony(colonyParameters);

            WaitForIsColonyCreatedMessage();

            m_ServiceProxy.Start();
        }

        private void WaitForIsColonyCreatedMessage()
        {
            SleepWaitAndDo(() => m_ServiceProxy.IsColonyCreated,
                           () => m_Logger.Info("Waiting for response 'IsColonyCreated'..."));
        }

        internal void SleepWaitAndDo([NotNull] Func <bool> breakIfTrue,
                                     [NotNull] Action doSomething)
        {
            for ( var i = 0 ; i < 10 ; i++ )
            {
                Thread.Sleep(SleepTimeOneSecond);

                if ( breakIfTrue() )
                {
                    break;
                }

                doSomething();
            }
        }
    }
}