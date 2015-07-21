using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;

namespace Selkie.WPF.Models.Control.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    internal sealed class ControlModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();

            m_Model = new ControlModel(m_Logger,
                                       m_Bus);
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private ControlModel m_Model;

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
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => x.IsApplying));
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
        public void ColonyFinishedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.ColonyFinishedHandler(new ColonyFinishedMessage());

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsRunning &&
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
        public void ColonyLinesChangedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Model.ColonyLinesChangedHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsApplying));
        }

        [Test]
        public void ColonyLinesChangedHandler_SetsIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Model.ColonyLinesChangedHandler(message);

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
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => x.IsRunning &&
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
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsRunning &&
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
        public void ColonyTestLineResponseHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyTestLinesChangedMessage();

            m_Model.ColonyTestLineResponseHandler(message);

            // Act
            m_Model.Apply();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelChangedMessage>(x => !x.IsApplying));
        }

        [Test]
        public void ColonyTestLineResponseHandler_SetIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            var message = new ColonyTestLinesChangedMessage();

            // Act
            m_Model.ColonyTestLineResponseHandler(message);

            // Assert
            Assert.False(m_Model.IsApplying);
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
            m_Model.ColonyTestLinesResponseHandler(new ColonyTestLinesResponseMessage
                                                   {
                                                       Types = types
                                                   });

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelTestLinesChangedMessage>(x => x.TestLineTypes == types));
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
            m_Model.ColonyTestLinesResponseHandler(new ColonyTestLinesResponseMessage
                                                   {
                                                       Types = types
                                                   });

            // Assert
            Assert.AreEqual(m_Model.TestLineTypes,
                            types);
        }

        [Test]
        public void Constructor_SendsColonyTestLinesRequestMessage_WhenCreated()
        {
            m_Bus.Received().PublishAsync(Arg.Any <ColonyTestLinesRequestMessage>());
        }

        [Test]
        public void Constructor_SubscribeToColonyFinishedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyFinishedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyLinesChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyLinesChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyStartedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyStartedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyStoppedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyStoppedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyTestLineResponseMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyTestLinesChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyTestLinesResponseMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyTestLinesResponseMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToControlModelTestLinesRequestMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ControlModelTestLineSetMessage, Task>>());
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
            m_Bus.Received().PublishAsync(Arg.Any <ColonyTestLinesRequestMessage>());
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