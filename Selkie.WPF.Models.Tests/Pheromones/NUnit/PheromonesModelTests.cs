using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Common;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Pheromones;

namespace Selkie.WPF.Models.Tests.Pheromones.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PheromonesModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_MemoryBus = Substitute.For <ISelkieInMemoryBus>();
            m_Timer = Substitute.For <ITimer>();

            m_Model = new PheromonesModel(m_Bus,
                                          m_MemoryBus,
                                          m_Timer);
        }

        private PheromonesModel m_Model;
        private ISelkieBus m_Bus;
        private ITimer m_Timer;
        private ISelkieInMemoryBus m_MemoryBus;

        private ColonyPheromonesMessage CreatePheromonesMessage()
        {
            var message = new ColonyPheromonesMessage
                          {
                              Values = new[]
                                       {
                                           new[]
                                           {
                                               0.1,
                                               0.2
                                           },
                                           new[]
                                           {
                                               0.3,
                                               0.4
                                           }
                                       },
                              Minimum = 0.6,
                              Maximum = 0.8,
                              Average = 0.7
                          };

            return message;
        }

        private ColonyFinishedMessage CreateFinishedMessage()
        {
            return new ColonyFinishedMessage
                   {
                       Times = 1,
                       StartTime = DateTime.Now,
                       EndTime = DateTime.Now
                   };
        }

        [Test]
        public void CallsInitializeOnTimerTest()
        {
            m_Timer.Received().Initialize(m_Model.OnTimer,
                                          PheromonesModel.TenSeconds,
                                          PheromonesModel.TwoSeconds);
        }

        [Test]
        public void DefaultAverageTest()
        {
            Assert.AreEqual(0.0,
                            m_Model.Average);
        }

        [Test]
        public void DefaultIsRequestingEnabledTest()
        {
            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void DefaultMaximumTest()
        {
            Assert.AreEqual(0.0,
                            m_Model.Maximum);
        }

        [Test]
        public void DefaultMinimumTest()
        {
            Assert.AreEqual(0.0,
                            m_Model.Minimum);
        }

        [Test]
        public void DefaultValuesTest()
        {
            Assert.NotNull(m_Model.Values);
        }

        [Test]
        public void FinishedHandlerUpdatesIsRequestingEnabledTest()
        {
            ColonyFinishedMessage message = CreateFinishedMessage();

            m_Model.FinishedHandler(message);

            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void OnTimerDoesNotSendsRequestMessageForRequestingDisabledTest()
        {
            m_Model.OnTimer(new object());

            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyPheromonesRequestMessage>());
        }

        [Test]
        public void OnTimerSendsRequestMessageTest()
        {
            m_Model.StartedHandler(new ColonyStartedMessage());

            m_Bus.ClearReceivedCalls();

            m_Model.OnTimer(new object());

            m_Bus.Received().PublishAsync(Arg.Any <ColonyPheromonesRequestMessage>());
        }

        [Test]
        public void PheromonesMessageHandlerSendsMessageTest()
        {
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Bus.ClearReceivedCalls();

            m_Model.PheromonesHandler(message);

            m_MemoryBus.Received()
                       .PublishAsync(Arg.Any <PheromonesModelChangedMessage>());
        }

        [Test]
        public void PheromonesMessageHandlerUpdatesAverageTest()
        {
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Model.PheromonesHandler(message);

            Assert.AreEqual(message.Average,
                            m_Model.Average);
        }

        [Test]
        public void PheromonesMessageHandlerUpdatesMaximumTest()
        {
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Model.PheromonesHandler(message);

            Assert.AreEqual(message.Maximum,
                            m_Model.Maximum);
        }

        [Test]
        public void PheromonesMessageHandlerUpdatesMinimumTest()
        {
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Model.PheromonesHandler(message);

            Assert.AreEqual(message.Minimum,
                            m_Model.Minimum);
        }

        [Test]
        public void PheromonesMessageHandlerUpdatesValuesTest()
        {
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Model.PheromonesHandler(message);

            Assert.AreEqual(message.Values,
                            m_Model.Values);
        }

        [Test]
        public void StartedHandlerSendsMessageTest()
        {
            var message = new ColonyStartedMessage();

            m_Model.StartedHandler(message);

            Assert.True(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void StoppedHandlerSendsMessageTest()
        {
            var message = new ColonyStoppedMessage();

            m_Model.StoppedHandler(message);

            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void SubscribeToColonyStartedMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyStartedMessage>>());
        }

        [Test]
        public void SubscribeToColonyStoppedMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyStoppedMessage>>());
        }

        [Test]
        public void SubscribeToFinishedMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyFinishedMessage>>());
        }

        [Test]
        public void SubscribeToPheromonesMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyPheromonesMessage>>());
        }
    }
}