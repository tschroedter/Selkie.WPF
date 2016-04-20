using System;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Control;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Tests.NUnit;

namespace Selkie.WPF.ViewModels.Tests.Control.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    internal sealed class ControlViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_ControlModel = Substitute.For <IControlModel>();
            m_Manager = Substitute.For <ICommandManager>();

            m_Model = CreateModel(m_Dispatcher);
        }

        private ControlViewModel CreateModel([NotNull] IApplicationDispatcher dispatcher)
        {
            return new ControlViewModel(m_Bus,
                                        dispatcher,
                                        m_ControlModel,
                                        m_Manager);
        }

        private ISelkieInMemoryBus m_Bus;
        private ControlViewModel m_Model;
        private TestImmediateDispatcher m_Dispatcher;
        private IControlModel m_ControlModel;
        private ICommandManager m_Manager;

        private void SetIsRunningToTrue(ControlViewModel model)
        {
            SendMessageToSetIsRunning(model,
                                      true);
        }

        private void SetIsLinesWereAppliedToTrue(ControlViewModel model)
        {
            m_Model.SelectedTestLine = "Something";

            var message = new ControlModelChangedMessage();

            model.ControlModelChangedHandler(message);
        }

        private static void SendMessageToSetIsRunning(ControlViewModel model,
                                                      bool isRunning)
        {
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = isRunning
                          };

            model.ControlModelChangedHandler(message);
        }

        private void SetIsNotRunningToTrue(ControlViewModel model)
        {
            SendMessageToSetIsRunning(model,
                                      false);
        }

        [Test]
        public void ApplyCommand_IsNotNull()
        {
            Assert.NotNull(m_Model.ApplyCommand);
        }

        [Test]
        public void ApplyCommandExitCommand_IsNotNull()
        {
            Assert.NotNull(m_Model.ExitCommand);
        }

        [Test]
        public void CanExecuteApplyCommand_ReturnsFalse_WhenIsApplyingIsTrue()
        {
            // Arrange
            m_Model.SelectedTestLine = "Hello";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = true
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.CanExecuteApplyCommand());
        }

        [Test]
        public void CanExecuteApplyCommand_ReturnsFalse_WhenIsRunningIsTrue()
        {
            // Arrange
            m_Model.SelectedTestLine = "Hello";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = true,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.CanExecuteApplyCommand());
        }

        [Test]
        public void CanExecuteApplyCommand_ReturnsFalse_WhenSelectedTestLineIsEmpty()
        {
            // Arrange
            m_Model.SelectedTestLine = "";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.CanExecuteApplyCommand());
        }

        [Test]
        public void CanExecuteApplyCommand_ReturnsFalse_WhenSelectedTestLineIsNull()
        {
            // Arrange
            m_Model.SelectedTestLine = null;
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.CanExecuteApplyCommand());
        }

        [Test]
        public void CanExecuteApplyCommand_ReturnsTrue()
        {
            // Arrange
            m_Model.SelectedTestLine = "Hello";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.True(m_Model.CanExecuteApplyCommand());
        }

        [Test]
        public void CanExecuteExitCommand_ReturnsTrue_WhenCalled()
        {
            Assert.True(m_Model.CanExecuteExitCommand());
        }

        [Test]
        public void CanExecuteStartCommand_ReturnsFalse_WhenIsLinesWereAppliedIsFalse()
        {
            // Arrange
            SetIsRunningToTrue(m_Model);

            // Act
            m_Model.CanExecuteStartCommand();

            // Assert
            Assert.False(m_Model.CanExecuteStartCommand());
        }

        [Test]
        public void CanExecuteStartCommand_ReturnsFalse_WhenIsRunningIsTrue()
        {
            // Arrange
            SetIsLinesWereAppliedToTrue(m_Model);
            SetIsRunningToTrue(m_Model);

            // Act
            m_Model.CanExecuteStartCommand();

            // Assert
            Assert.False(m_Model.CanExecuteStartCommand());
        }

        [Test]
        public void CanExecuteStartCommand_ReturnsTrue_WhenIsRunningIsFalse()
        {
            // Arrange
            SetIsLinesWereAppliedToTrue(m_Model);
            SetIsNotRunningToTrue(m_Model);

            // Act
            m_Model.CanExecuteStartCommand();

            // Assert
            Assert.True(m_Model.CanExecuteStartCommand());
        }

        [Test]
        public void CanExecuteStopCommand_ReturnsFalse_WhenIsRunningIsFalse()
        {
            // Arrange
            SetIsNotRunningToTrue(m_Model);

            // Act
            m_Model.CanExecuteStopCommand();

            // Assert
            Assert.False(m_Model.CanExecuteStopCommand());
        }

        [Test]
        public void CanExecuteStopCommand_ReturnsTrue_WhenIsRunningIsTrue()
        {
            // Arrange
            SetIsRunningToTrue(m_Model);

            // Act
            m_Model.CanExecuteStopCommand();

            // Assert
            Assert.True(m_Model.CanExecuteStopCommand());
        }

        [Test]
        public void Constructor_SubscribeToControlModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ControlModelChangedMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToControlModelTestLinesResponseMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ControlModelTestLinesResponseMessage>>());
        }

        [Test]
        public void ControlModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            ControlViewModel model = CreateModel(dispatcher);
            var message = new ControlModelChangedMessage();

            // Act
            model.ControlModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.Update);
        }

        [Test]
        public void ControlModelChangedHandler_SetsIsApplying_WhenCalled()
        {
            // Arrange
            var message = new ControlModelChangedMessage
                          {
                              IsApplying = true
                          };

            // Act
            m_Model.ControlModelChangedHandler(message);

            // Assert
            Assert.True(m_Model.IsApplying);
        }

        [Test]
        public void ControlModelChangedHandler_SetsIsLinesWereAppliedToFalse_WhenSelectedTestLineIsInvalid()
        {
            // Arrange
            m_Model.SelectedTestLine = string.Empty;

            var message = new ControlModelChangedMessage();

            // Act
            m_Model.ControlModelChangedHandler(message);

            // Assert
            Assert.False(m_Model.IsLinesWereApplied);
        }

        [Test]
        public void ControlModelChangedHandler_SetsIsLinesWereAppliedToTrue_WhenSelectedTestLineIsValid()
        {
            // Arrange
            m_Model.SelectedTestLine = "Something";

            var message = new ControlModelChangedMessage();

            // Act
            m_Model.ControlModelChangedHandler(message);

            // Assert
            Assert.True(m_Model.IsLinesWereApplied);
        }

        [Test]
        public void ControlModelChangedHandler_SetsIsRunning_WhenCalled()
        {
            // Arrange
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = true
                          };

            // Act
            m_Model.ControlModelChangedHandler(message);

            // Assert
            Assert.True(m_Model.IsRunning);
        }

        [Test]
        public void ControlModelTestLinesResponseHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            ControlViewModel model = CreateModel(dispatcher);
            var message = new ControlModelTestLinesResponseMessage();

            // Act
            model.ControlModelTestLinesResponseHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.Update);
        }

        [Test]
        public void ControlModelTestLinesResponseHandler_SetsTestLines_WhenCalled()
        {
            // Arrange
            var types = new[]
                        {
                            "Test Line 1"
                        };

            var message = new ControlModelTestLinesResponseMessage
                          {
                              TestLineTypes = types
                          };

            // Act
            m_Model.ControlModelTestLinesResponseHandler(message);

            // Assert
            Assert.True(types.SequenceEqual(m_Model.TestLines));
        }

        [Test]
        public void IsApplyEnabled_ReturnsFalse_WhenIsApplyingIsTrue()
        {
            // Arrange
            m_Model.SelectedTestLine = "Hello";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = true
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.IsApplyEnabled);
        }

        [Test]
        public void IsApplyEnabled_ReturnsFalse_WhenIsRunningIsTrue()
        {
            // Arrange
            m_Model.SelectedTestLine = "Hello";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = true,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.IsApplyEnabled);
        }

        [Test]
        public void IsApplyEnabled_ReturnsFalse_WhenSelectedTestLineIsEmpty()
        {
            // Arrange
            m_Model.SelectedTestLine = "";
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.IsApplyEnabled);
        }

        [Test]
        public void IsApplyEnabled_ReturnsFalse_WhenSelectedTestLineIsNull()
        {
            // Arrange
            m_Model.SelectedTestLine = null;
            var message = new ControlModelChangedMessage
                          {
                              IsRunning = false,
                              IsApplying = false
                          };

            m_Model.ControlModelChangedHandler(message);

            // Act
            // Assert
            Assert.False(m_Model.IsApplyEnabled);
        }

        [Test]
        public void IsExitEnabled_ReturnsTrue_WhenCalled()
        {
            Assert.True(m_Model.IsExitEnabled);
        }

        [Test]
        public void IsStartEnabled_ReturnsFalse_WhenIsRunningIsTrue()
        {
            // Arrange
            SetIsLinesWereAppliedToTrue(m_Model);
            SetIsRunningToTrue(m_Model);

            // Act
            // Assert
            Assert.False(m_Model.IsStartEnabled);
        }

        [Test]
        public void IsStartEnabled_ReturnsTrue_WhenIsRunningIsFalse()
        {
            // Arrange
            SetIsLinesWereAppliedToTrue(m_Model);
            SetIsNotRunningToTrue(m_Model);

            // Act
            // Assert
            Assert.True(m_Model.IsStartEnabled);
        }

        [Test]
        public void IsStopEnabled_ReturnsFalse_WhenIsRunningIsFalse()
        {
            // Arrange
            SetIsNotRunningToTrue(m_Model);

            // Act
            // Assert
            Assert.False(m_Model.IsStopEnabled);
        }

        [Test]
        public void IsStopEnabled_ReturnsTrue_WhenIsRunningIsTrue()
        {
            // Arrange
            SetIsRunningToTrue(m_Model);

            // Act
            // Assert
            Assert.True(m_Model.IsStopEnabled);
        }

        [Test]
        public void SelectedTestLine_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            ControlViewModel model = CreateModel(dispatcher);

            // Act
            model.SelectedTestLine = "Hello";

            // Assert
            dispatcher.Received().BeginInvoke(model.Update);
        }

        [Test]
        public void SelectedTestLine_SendsMessage_WhenSetGetCalled()
        {
            // Arrange
            // Act
            m_Model.SelectedTestLine = "Hello";

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <ControlModelTestLineSetMessage>(x => x.Type.Equals("Hello")));
        }

        [Test]
        public void SelectedTestLine_SetsIsLinesWereAppliedToFalse_WhenSetGetCalled()
        {
            // Arrange
            // Act
            m_Model.SelectedTestLine = "Hello";

            // Assert
            Assert.False(m_Model.IsLinesWereApplied);
        }

        [Test]
        public void SendsMessage_WhenCreated()
        {
            m_Bus.Received().PublishAsync(Arg.Any <ControlModelTestLinesRequestMessage>());
        }

        [Test]
        public void StartCommand_IsNotNull()
        {
            Assert.NotNull(m_Model.StartCommand);
        }

        [Test]
        public void StopCommand_IsNotNull()
        {
            Assert.NotNull(m_Model.StopCommand);
        }

        [Test]
        public void Update_CallsCommandManager_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Update();

            // Assert
            m_Manager.Received().InvalidateRequerySuggested();
        }

        [Test]
        public void Update_NotifiesIsApplyEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "IsApplyEnabled");

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void Update_NotifiesIsStartEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "IsStartEnabled");

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void Update_NotifiesIsStopEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "IsStopEnabled");

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void Update_NotifiesSelectedTestLines_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "SelectedTestLines");

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void Update_NotifiesTestLines_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "TestLines");

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }
    }
}