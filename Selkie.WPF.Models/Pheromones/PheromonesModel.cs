using JetBrains.Annotations;
using Selkie.Common;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Pheromones
{
    public class PheromonesModel : IPheromonesModel
    {
        internal const int TwoSeconds = 2000;
        internal const int TenSeconds = 10000;
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly object m_Padlock = new object();

        public PheromonesModel([NotNull] ISelkieInMemoryBus bus,
                               [NotNull] ITimer timer)
        {
            m_Bus = bus;

            Values = new[]
                     {
                         new[]
                         {
                             0.0,
                             1.0
                         }
                     };

            Minimum = 0.0;
            Maximum = 1.0;
            Average = 0.5;

            string subscriptionId = GetType().ToString();

            m_Bus.SubscribeAsync <ColonyStartedMessage>(subscriptionId,
                                                        StartedHandler);
            m_Bus.SubscribeAsync <ColonyStoppedMessage>(subscriptionId,
                                                        StoppedHandler);
            m_Bus.SubscribeAsync <ColonyPheromonesMessage>(subscriptionId,
                                                           PheromonesHandler);
            m_Bus.SubscribeAsync <ColonyFinishedMessage>(subscriptionId,
                                                         FinishedHandler);

            m_Bus.SubscribeAsync <PheromonesModelsSetMessage>(subscriptionId,
                                                              SetHandler);

            timer.Initialize(OnTimer,
                             TenSeconds,
                             TwoSeconds);
        }

        public bool IsRequestingEnabled { get; private set; }
        public double[][] Values { get; internal set; }
        public double Minimum { get; internal set; }
        public double Maximum { get; internal set; }
        public double Average { get; internal set; }
        public bool IsShowPheromones { get; internal set; }

        internal void OnTimer(object state)
        {
            if ( !IsShowPheromones ||
                 !IsRequestingEnabled )
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

            m_Bus.PublishAsync(new PheromonesModelChangedMessage()); // todo better to include parameters???
        }

        internal void FinishedHandler(ColonyFinishedMessage message)
        {
            IsRequestingEnabled = false;
        }

        internal void SetHandler(PheromonesModelsSetMessage message) // todo testing
        {
            IsShowPheromones = message.IsShowPheromones;

            m_Bus.PublishAsync(new PheromonesModelChangedMessage());
        }
    }
}