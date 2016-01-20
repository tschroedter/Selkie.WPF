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
        private readonly ISelkieBus m_Bus;
        private readonly ISelkieLogger m_Logger;
        private readonly ISelkieInMemoryBus m_MemoryBus;

        public ControlModel([NotNull] ISelkieLogger logger,
                            [NotNull] ISelkieBus bus,
                            [NotNull] ISelkieInMemoryBus memoryBus)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_MemoryBus = memoryBus;

            IsFinished = false;
            IsRunning = false;

            string subscriptionId = GetType().FullName;

            m_Bus.SubscribeAsync <ColonyStartedMessage>(subscriptionId,
                                                        ColonyStartedHandler);

            m_Bus.SubscribeAsync <ColonyStoppedMessage>(subscriptionId,
                                                        ColonyStoppedHandler);

            m_Bus.SubscribeAsync <ColonyFinishedMessage>(subscriptionId,
                                                         ColonyFinishedHandler);

            m_Bus.SubscribeAsync <ColonyTestLinesResponseMessage>(subscriptionId,
                                                                  ColonyTestLinesResponseHandler);

            m_Bus.SubscribeAsync <ColonyLinesChangedMessage>(subscriptionId,
                                                             ColonyLinesChangedHandler);

            m_Bus.SubscribeAsync <ControlModelTestLinesRequestMessage>(subscriptionId,
                                                                       ControlModelTestLinesRequestHandler);

            m_Bus.SubscribeAsync <ColonyTestLinesChangedMessage>(subscriptionId,
                                                                 ColonyTestLineResponseHandler);

            m_MemoryBus.SubscribeAsync <ControlModelTestLineSetMessage>(subscriptionId,
                                                                        ControlModelTestLineSetHandler);

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
            m_MemoryBus.PublishAsync(new ControlModelChangedMessage
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

            m_MemoryBus.PublishAsync(new ControlModelTestLinesChangedMessage
                                     {
                                         TestLineTypes = message.Types
                                     });
        }
    }
}