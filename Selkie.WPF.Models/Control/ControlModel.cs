using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Control
{
    public class ControlModel : IControlModel
    {
        public ControlModel([NotNull] ISelkieLogger logger,
                            [NotNull] ISelkieInMemoryBus bus)
        {
            m_Logger = logger;
            m_Bus = bus;

            IsFinished = false;
            IsRunning = false;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <ColonyStartedMessage>(subscriptionId,
                                                        ColonyStartedHandler);

            m_Bus.SubscribeAsync <ColonyStoppedMessage>(subscriptionId,
                                                        ColonyStoppedHandler);

            m_Bus.SubscribeAsync <ColonyFinishedMessage>(subscriptionId,
                                                         ColonyFinishedHandler);

            m_Bus.SubscribeAsync <ColonyAvailableTestLinesResponseMessage>(subscriptionId,
                                                                           ColonyAvailableTestLinesResponseHandler);

            m_Bus.SubscribeAsync <ColonyLinesResponseMessage>(subscriptionId,
                                                              ColonyLinesResponsedHandler);

            m_Bus.SubscribeAsync <ControlModelTestLinesRequestMessage>(subscriptionId,
                                                                       ControlModelTestLinesRequestHandler);

            m_Bus.SubscribeAsync <ColonyLineResponseMessage>(subscriptionId,
                                                             ColonyTestLineResponseHandler);

            m_Bus.SubscribeAsync <ControlModelTestLineSetMessage>(subscriptionId,
                                                                  ControlModelTestLineSetHandler);

            m_Bus.PublishAsync(new ColonyAvailabeTestLinesRequestMessage());
        }

        public string SelectedTestLine { get; private set; }
        public bool IsFinished { get; private set; }
        public IEnumerable <string> TestLineTypes { get; private set; }
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ISelkieLogger m_Logger;
        public bool IsApplying { get; private set; }

        public void Start()
        {
            if ( IsRunning )
            {
                m_Logger.Warn("Already running!");
                return;
            }

            m_Bus.PublishAsync(new ColonyStartRequestMessage());
        }

        public void Stop()
        {
            if ( !IsRunning )
            {
                m_Logger.Warn("Nothing is running!");
                return;
            }

            m_Bus.PublishAsync(new ColonyStopRequestMessage());
        }

        public void Apply()
        {
            IsApplying = true;

            m_Bus.PublishAsync(new ColonyTestLineSetMessage
                               {
                                   Type = SelectedTestLine
                               });

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        public bool IsRunning { get; private set; }

        internal void ColonyAvailableTestLinesResponseHandler(ColonyAvailableTestLinesResponseMessage message)
        {
            TestLineTypes = message.Types;

            m_Bus.PublishAsync(new ControlModelTestLinesResponseMessage
                               {
                                   TestLineTypes = message.Types
                               });
        }

        internal void ColonyFinishedHandler(ColonyFinishedMessage message)
        {
            IsRunning = false;
            IsFinished = true;
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ColonyLinesResponsedHandler(ColonyLinesResponseMessage message)
        {
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ColonyStartedHandler(ColonyStartedMessage message)
        {
            IsRunning = true;
            IsFinished = false;
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ColonyStoppedHandler(ColonyStoppedMessage message)
        {
            IsRunning = false;
            IsFinished = false;
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ColonyTestLineResponseHandler(ColonyLineResponseMessage message)
        {
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ControlModelTestLineSetHandler(ControlModelTestLineSetMessage message)
        {
            SelectedTestLine = message.Type;
        }

        internal void ControlModelTestLinesRequestHandler(ControlModelTestLinesRequestMessage obj)
        {
            m_Bus.PublishAsync(new ColonyAvailabeTestLinesRequestMessage());
        }

        private void SendChangedMessage(bool isRunning,
                                        bool isFinished,
                                        bool isApplying)
        {
            m_Bus.PublishAsync(new ControlModelChangedMessage
                               {
                                   IsRunning = isRunning,
                                   IsFinished = isFinished,
                                   IsApplying = isApplying
                               });

            m_Logger.Info("IsRunning: {0} IsFinished: {1}".Inject(isRunning,
                                                                  isFinished));
        }
    }
}