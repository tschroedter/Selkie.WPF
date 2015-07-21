using Castle.Core.Logging;
using EasyNetQ;
using Selkie.Common;
using Selkie.EasyNetQ.Extensions;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Pheromones
{
    public class PheromonesModel : IPheromonesModel
    {
        internal const int TwoSeconds = 2000;
        internal const int TenSeconds = 10000;
        private readonly IBus m_Bus;
        private readonly object m_Padlock = new object();

        public PheromonesModel(ILogger logger,
                               IBus bus,
                               ITimer timer)
        {
            m_Bus = bus;

            Values = new double[0][];
            Minimum = 0.0;
            Maximum = 0.0;
            Average = 0.0;

            string subscriptionId = GetType().ToString();

            bus.SubscribeHandlerAsync <ColonyStartedMessage>(logger,
                                                             subscriptionId,
                                                             StartedHandler);
            bus.SubscribeHandlerAsync <ColonyStoppedMessage>(logger,
                                                             subscriptionId,
                                                             StoppedHandler);
            bus.SubscribeHandlerAsync <ColonyPheromonesMessage>(logger,
                                                                subscriptionId,
                                                                PheromonesHandler);
            bus.SubscribeHandlerAsync <ColonyFinishedMessage>(logger,
                                                              subscriptionId,
                                                              FinishedHandler);

            timer.Initialize(OnTimer,
                             TenSeconds,
                             TwoSeconds);
        }

        public bool IsRequestingEnabled { get; private set; }
        public double[][] Values { get; internal set; }
        public double Minimum { get; internal set; }
        public double Maximum { get; internal set; }
        public double Average { get; internal set; }

        internal void OnTimer(object state)
        {
            if ( !IsRequestingEnabled )
            {
                return;
            }

            m_Bus.PublishAsync(new ColonyPheromonesRequestMessage());
        }

        internal void StartedHandler(ColonyStartedMessage message)
        {
            IsRequestingEnabled = true;
        }

        internal void StoppedHandler(ColonyStoppedMessage message)
        {
            IsRequestingEnabled = false;
        }

        internal void PheromonesHandler(ColonyPheromonesMessage message)
        {
            lock ( m_Padlock )
            {
                Values = message.Values;
                Minimum = message.Minimum;
                Maximum = message.Maximum;
                Average = message.Average;
            }

            m_Bus.PublishAsync(new PheromonesModelChangedMessage());
        }

        internal void FinishedHandler(ColonyFinishedMessage message)
        {
            IsRequestingEnabled = false;
        }
    }
}