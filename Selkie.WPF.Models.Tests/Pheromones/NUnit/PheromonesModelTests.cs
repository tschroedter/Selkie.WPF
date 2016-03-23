using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Common;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.NUnit.Extensions;
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

        private void SetIsIsRequestingEnabledToTrue()
        {
            m_Model.StartedHandler(new ColonyStartedMessage());
        }

        private void SetIsShowPheromones(bool value)
        {
            m_Model.SetHandler(new PheromonesModelsSetMessage
                               {
                                   IsShowPheromones = value
                               });
        }

        [Test]
        public void Average_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0.5,
                            m_Model.Average);
        }

        [Test]
        public void Constructor_CallsInitializeOnTimer_WhenCreated()
        {
            // Arrange
            // Act
            // Assert
            m_Timer.Received().Initialize(m_Model.OnTimer,
                                          PheromonesModel.TenSeconds,
                                          PheromonesModel.TwoSeconds);
        }

        [Test]
        public void Constructor_SubscribesToColonyStartedMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyStartedMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToFinishedMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyFinishedMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToPheromonesMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyPheromonesMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToPheromonesModelsSetMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_MemoryBus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                                  Arg.Any <Action <PheromonesModelsSetMessage>>());
        }

        [Test]
        public void Constructor_SubscribeTsoColonyStoppedMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(m_Model.GetType().ToString(),
                                            Arg.Any <Action <ColonyStoppedMessage>>());
        }

        [Test]
        public void FinishedHandlerUpdatesIsRequestingEnabledTest()
        {
            // Act
            ColonyFinishedMessage message = CreateFinishedMessage();

            // Arrange
            m_Model.FinishedHandler(message);

            // Assert
            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void IsRequestingEnabled_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void IsShowPheromones_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Model.IsShowPheromones);
        }

        [Test]
        public void Maximum_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(1.0,
                            m_Model.Maximum);
        }

        [Test]
        public void Minimum_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0.0,
                            m_Model.Minimum);
        }

        [Test]
        public void OnTimer_DoesNotSendsRequestMessage_ForIsRequestingFalseAndIsShowPheromonesTrue()
        {
            // Arrange
            SetIsShowPheromones(true);

            // Act
            m_Model.OnTimer(new object());

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyPheromonesRequestMessage>());
        }

        [Test]
        public void OnTimer_DoesNotSendsRequestMessage_ForIsShowPheromonesIsFalse()
        {
            // Arrange
            SetIsShowPheromones(false);
            SetIsIsRequestingEnabledToTrue();

            m_Bus.ClearReceivedCalls();

            // Act
            m_Model.OnTimer(new object());

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyPheromonesRequestMessage>());
        }

        [Test]
        public void OnTimer_SendsRequestMessage_WhenCalledEnabled()
        {
            // Arrange
            SetIsShowPheromones(true);
            SetIsIsRequestingEnabledToTrue();

            m_Bus.ClearReceivedCalls();

            // Act
            m_Model.OnTimer(new object());

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyPheromonesRequestMessage>());
        }

        [Test]
        public void PheromonesMessageHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            m_Bus.ClearReceivedCalls();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            m_MemoryBus.Received()
                       .PublishAsync(Arg.Any <PheromonesModelChangedMessage>());
        }

        [Test]
        public void PheromonesMessageHandler_UpdatesAverage_WhenCalled()
        {
            // Arrange
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            Assert.AreEqual(message.Average,
                            m_Model.Average);
        }

        [Test]
        public void PheromonesMessageHandler_UpdatesMaximum_WhenCalled()
        {
            // Arrange
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            Assert.AreEqual(message.Maximum,
                            m_Model.Maximum);
        }

        [Test]
        public void PheromonesMessageHandler_UpdatesMinimum_WhenCalled()
        {
            // Arrange
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            Assert.AreEqual(message.Minimum,
                            m_Model.Minimum);
        }

        [Test]
        public void PheromonesMessageHandler_UpdatesValues_WhenCalled()
        {
            // Arrange
            ColonyPheromonesMessage message = CreatePheromonesMessage();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            Assert.AreEqual(message.Values,
                            m_Model.Values);
        }

        [Test]
        public void StartedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyStartedMessage();

            // Act
            m_Model.StartedHandler(message);

            // Assert
            Assert.True(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void StoppedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyStoppedMessage();

            // Act
            m_Model.StoppedHandler(message);

            // Assert
            Assert.False(m_Model.IsRequestingEnabled);
        }

        [Test]
        public void Values_ReturnsValue_WhenCalled()
        {
            // Arrange
            var expected = new[]
                           {
                               new[]
                               {
                                   0.0,
                                   1.0
                               }
                           };

            // Act
            double[][] actual = m_Model.Values;

            // Assert
            Assert.AreEqual(expected.Length,
                            actual.Length,
                            "Length");

            NUnitHelper.AssertSequenceEqual(expected [ 0 ],
                                            actual [ 0 ],
                                            "[0]");
        }
    }
}