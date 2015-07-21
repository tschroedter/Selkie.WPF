using System;
using System.Threading;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Services.Aco.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public sealed class Colony : IColony
    {
        internal const int SleepTimeOneSecond = 1000;
        private readonly IBus m_Bus;
        private readonly ILogger m_Logger;
        private readonly IServiceProxy m_ServiceProxy;

        public Colony([NotNull] ILogger logger,
                      [NotNull] IBus bus,
                      [NotNull] IServiceProxy serviceProxy)
        {
            SleepTimeInMs = SleepTimeOneSecond;
            m_Logger = logger;
            m_Bus = bus;
            m_ServiceProxy = serviceProxy;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeHandlerAsync <ColonyStartRequestMessage>(logger,
                                                                    subscriptionId,
                                                                    ColonyStartRequestHandler);

            m_Bus.SubscribeHandlerAsync <ColonyStopRequestMessage>(logger,
                                                                   subscriptionId,
                                                                   ColonyStopRequestHandler);

            m_Bus.SubscribeHandlerAsync <ColonyPheromonesRequestMessage>(logger,
                                                                         subscriptionId,
                                                                         ColonyPheromonesRequestHandler);
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
            m_ServiceProxy.CreateColony();
            WaitForIsColonyCreatedMessage();
            m_ServiceProxy.Start();
        }

        internal void ColonyStopRequestHandler(ColonyStopRequestMessage message)
        {
            m_ServiceProxy.Stop();
        }

        internal void ColonyPheromonesRequestHandler(ColonyPheromonesRequestMessage message)
        {
            m_Bus.PublishAsync(new PheromonesRequestMessage());
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