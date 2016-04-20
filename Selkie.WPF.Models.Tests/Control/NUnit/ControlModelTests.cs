using System;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Control;

namespace Selkie.WPF.Models.Tests.Control.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    internal sealed class ControlModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();

            m_Model = new ControlModel(m_Logger,
                                       m_Bus);
        }

        private ISelkieLogger m_Logger;
        private ControlModel m_Model;
        private ISelkieInMemoryBus m_Bus;

        [Test]
        public void Apply_SendsColonyTestLineSetMessage_WhenCalled()
        {
            // Arrange
            var message = new ControlModelTestLineSetMessage
                          {
                              Type = "Test"
                          };
            m_Model.ControlModelTestLineSetHandler(message);

            // Act
            m_Model.Apply();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ColonyTestLineSetMessage>(x => x.Type == "Test"));
        }

        [Test]
        public void Apply_SendsControlModelChangedMessage_WhenCalled()
        {
            // Arrange
            var message = new ControlModelTestLineSetMessage
                          {
                              Type = "Test"
                          };
            m_Model.ControlModelTestLineSetHandler(message);

            // Act
            m_Model.Apply();

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelChangedMessage>(x => x.IsApplying));
        }

        [Test]
        public void Apply_SetsIsApplyingToTrue_WhenCalled()
        {
            // Arrange
            var message = new ControlModelTestLineSetMessage
                          {
                              Type = "Test"
                          };
            m_Model.ControlModelTestLineSetHandler(message);

            // Act
            m_Model.Apply();

            // Assert
            Assert.True(m_Model.IsApplying);
        }

        [Test]
        public void ColonyAvailableTestLinesResponseHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyAvailableTestLinesResponseMessage();

            // Act
            m_Model.ColonyAvailableTestLinesResponseHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelTestLinesResponseMessage>(x => x.TestLineTypes.Equals(message.Types)));
        }

        [Test]
        public void ColonyAvailableTestLinesResponseHandler_SetIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            var message = new ColonyAvailableTestLinesResponseMessage();

            // Act
            m_Model.ColonyAvailableTestLinesResponseHandler(message);

            // Assert
            Assert.False(m_Model.IsApplying);
        }

        [Test]
        public void ColonyFinishedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ColonyFinishedHandler(new ColonyFinishedMessage());

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsRunning &&
                                                                        x.IsFinished &&
                                                                        !x.IsApplying));
        }

        [Test]
        public void ColonyFinishedHandler_SetsIsFinishedToTrue_WhenCalled()
        {
            // Arrange
            Assert.False(m_Model.IsFinished);

            // Act
            m_Model.ColonyFinishedHandler(new ColonyFinishedMessage());

            // Assert
            Assert.True(m_Model.IsFinished);
        }

        [Test]
        public void ColonyFinishedHandler_SetsIsRunningToFalse_WhenCalled()
        {
            // Arrange
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Act
            m_Model.ColonyFinishedHandler(new ColonyFinishedMessage());

            // Assert
            Assert.False(m_Model.IsRunning);
        }

        [Test]
        public void ColonyLinesResponsedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesResponseMessage();

            // Act
            m_Model.ColonyLinesResponsedHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsApplying));
        }

        [Test]
        public void ColonyLinesResponsedHandler_SetsIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesResponseMessage();

            // Act
            m_Model.ColonyLinesResponsedHandler(message);

            // Assert
            Assert.False(m_Model.IsApplying);
        }

        [Test]
        public void ColonyStartedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelChangedMessage>(x => x.IsRunning &&
                                                                        !x.IsFinished &&
                                                                        !x.IsApplying));
        }

        [Test]
        public void ColonyStartedHandler_SetIsFinishedToFalse_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Assert
            Assert.False(m_Model.IsFinished);
        }

        [Test]
        public void ColonyStartedHandler_SetIsRunningToTrue_WhenCalled()
        {
            // Arrange
            Assert.False(m_Model.IsRunning);

            // Act
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Assert
            Assert.True(m_Model.IsRunning);
        }

        [Test]
        public void ColonyStoppedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ColonyStoppedHandler(new ColonyStoppedMessage());

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsRunning &&
                                                                        !x.IsFinished &&
                                                                        !x.IsApplying));
        }

        [Test]
        public void ColonyStoppedHandler_SetIsFinishedToFalse_WhenCalled()
        {
            // Arrange
            m_Model.ColonyFinishedHandler(new ColonyFinishedMessage());

            // Act
            m_Model.ColonyStoppedHandler(new ColonyStoppedMessage());

            // Assert
            Assert.False(m_Model.IsFinished);
        }

        [Test]
        public void ColonyStoppedHandler_SetIsRunningToFalse_WhenCalled()
        {
            // Arrange
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Act
            m_Model.ColonyStoppedHandler(new ColonyStoppedMessage());

            // Assert
            Assert.False(m_Model.IsRunning);
        }

        [Test]
        public void ColonyTestLinesResponseHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var types = new[]
                        {
                            "one",
                            "two"
                        };

            // Act
            m_Model.ColonyAvailableTestLinesResponseHandler(new ColonyAvailableTestLinesResponseMessage
                                                            {
                                                                Types = types
                                                            });

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ControlModelTestLinesResponseMessage>(x => x.TestLineTypes == types));
        }

        [Test]
        public void ColonyTestLinesResponseHandler_SetsAvailableTypes_WhenCalled()
        {
            // Arrange
            var types = new[]
                        {
                            "one",
                            "two"
                        };

            // Act
            m_Model.ColonyAvailableTestLinesResponseHandler(new ColonyAvailableTestLinesResponseMessage
                                                            {
                                                                Types = types
                                                            });

            // Assert
            Assert.AreEqual(m_Model.TestLineTypes,
                            types);
        }

        [Test]
        public void Constructor_SendsColonyAvailableTestLinesResponseMessage_WhenCreated()
        {
            m_Bus.Received()
                 .PublishAsync(Arg.Any <ColonyAvailabeTestLinesRequestMessage>());
        }

        [Test]
        public void Constructor_SubscribeToColonyFinishedMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyFinishedMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyLinesResponseMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyLinesResponseMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyStartedMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyStartedMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyStoppedMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyStoppedMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyTestLineResponseMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyAvailableTestLinesResponseMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyTestLinesResponseMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ColonyAvailableTestLinesResponseMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToControlModelTestLinesRequestMessage_WhenCreated()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().FullName,
                                 Arg.Any <Action <ControlModelTestLineSetMessage>>());
        }

        [Test]
        public void ControlModelTestLineSetHandler_SetsSelectedTestLines_WhenCalled()
        {
            // Arrange
            var message = new ControlModelTestLineSetMessage
                          {
                              Type = "Test"
                          };

            // Act
            m_Model.ControlModelTestLineSetHandler(message);

            // Assert
            Assert.True(m_Model.SelectedTestLine == "Test");
        }

        [Test]
        public void ControlModelTestLinesRequestHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ControlModelTestLinesRequestHandler(new ControlModelTestLinesRequestMessage());

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyAvailabeTestLinesRequestMessage>());
        }

        [Test]
        public void Start_DoesNotSendColonyStartRequestMessage_WhenIsRunningIsTrue()
        {
            // Arrange
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Act
            m_Model.Start();

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyStartRequestMessage>());
        }

        [Test]
        public void Start_SendColonyStartRequestMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Start();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyStartRequestMessage>());
        }

        [Test]
        public void Stop_DoesNotSendColonyStartRequestMessage_ForIsRunningIsFalse()
        {
            // Arrange
            // Act
            m_Model.Stop();

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyStopRequestMessage>());
        }

        [Test]
        public void Stop_SendColonyStopRequestMessage_ForIsRunningIsTrue()
        {
            // Arrange
            m_Model.ColonyStartedHandler(new ColonyStartedMessage());

            // Act
            m_Model.Stop();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyStopRequestMessage>());
        }
    }
}