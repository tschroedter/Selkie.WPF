using System.Collections.Generic;
using Castle.Core.Logging;
using EasyNetQ;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Control
{
    public class ControlModel : IControlModel
    {
        private readonly IBus m_Bus;
        private readonly ILogger m_Logger;

        public ControlModel(ILogger logger,
                            IBus bus)
        {
            m_Logger = logger;
            m_Bus = bus;

            IsFinished = false;
            IsRunning = false;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeHandlerAsync <ColonyStartedMessage>(logger,
                                                               subscriptionId,
                                                               ColonyStartedHandler);

            m_Bus.SubscribeHandlerAsync <ColonyStoppedMessage>(logger,
                                                               subscriptionId,
                                                               ColonyStoppedHandler);

            m_Bus.SubscribeHandlerAsync <ColonyFinishedMessage>(logger,
                                                                subscriptionId,
                                                                ColonyFinishedHandler);

            m_Bus.SubscribeHandlerAsync <ColonyTestLinesResponseMessage>(logger,
                                                                         subscriptionId,
                                                                         ColonyTestLinesResponseHandler);

            m_Bus.SubscribeHandlerAsync <ColonyLinesChangedMessage>(logger,
                                                                    subscriptionId,
                                                                    ColonyLinesChangedHandler);

            m_Bus.SubscribeHandlerAsync <ControlModelTestLinesRequestMessage>(logger,
                                                                              subscriptionId,
                                                                              ControlModelTestLinesRequestHandler);

            m_Bus.SubscribeHandlerAsync <ControlModelTestLineSetMessage>(logger,
                                                                         subscriptionId,
                                                                         ControlModelTestLineSetHandler);

            m_Bus.SubscribeHandlerAsync <ColonyTestLinesChangedMessage>(logger,
                                                                        subscriptionId,
                                                                        ColonyTestLineResponseHandler);

            m_Bus.PublishAsync(new ColonyTestLinesRequestMessage());
        }

        public string SelectedTestLine { get; private set; }
        public bool IsFinished { get; private set; }
        public IEnumerable <string> TestLineTypes { get; private set; }
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

        internal void ColonyLinesChangedHandler(ColonyLinesChangedMessage message)
        {
            IsApplying = false;

            SendChangedMessage(IsRunning,
                               IsFinished,
                               IsApplying);
        }

        internal void ColonyTestLineResponseHandler(ColonyTestLinesChangedMessage message)
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
            m_Bus.PublishAsync(new ColonyTestLinesRequestMessage());
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

        internal void ColonyTestLinesResponseHandler(ColonyTestLinesResponseMessage message)
        {
            TestLineTypes = message.Types;

            m_Bus.PublishAsync(new ControlModelTestLinesChangedMessage
                               {
                                   TestLineTypes = message.Types
                               });
        }
    }
}