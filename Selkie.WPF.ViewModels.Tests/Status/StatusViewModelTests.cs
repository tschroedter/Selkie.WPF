using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Status;

namespace Selkie.WPF.ViewModels.Tests.Status
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class StatusViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_CommandManager = Substitute.For <ICommandManager>();
            m_StatusModel = Substitute.For <IStatusModel>();
            m_ExceptionThrownModel = Substitute.For <IExceptionThrownModel>();

            m_Sut = new StatusViewModel(m_Bus,
                                        m_Dispatcher,
                                        m_CommandManager,
                                        m_ExceptionThrownModel,
                                        m_StatusModel);
        }

        private ISelkieInMemoryBus m_Bus;
        private IApplicationDispatcher m_Dispatcher;
        private IStatusModel m_StatusModel;
        private StatusViewModel m_Sut;
        private IExceptionThrownModel m_ExceptionThrownModel;
        private ICommandManager m_CommandManager;

        [Test]
        public void CanExecuteClearErrorCommand_ReturnsFalse_ForExceptionThrownIsEmpty()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Sut.CanExecuteClearErrorCommand());
        }

        [Test]
        public void CanExecuteClearErrorCommand_ReturnsTrue_ForExceptionThrownIsNotEmpty()
        {
            // Arrange
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = "Text"
                          };

            m_Sut.ExceptionThrownChangedHandler(message);

            // Act
            // Assert
            Assert.True(m_Sut.CanExecuteClearErrorCommand());
        }

        [Test]
        public void ClearErrorCommand_IsNotNull()
        {
            Assert.NotNull(m_Sut.ClearErrorCommand);
        }

        [Test]
        public void ExceptionThrown_ReturnsEmpty_ByDefault()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.ExceptionThrown);
        }

        [Test]
        public void ExceptionThrownChangedHandler_RaisesEventExceptionThrown_WhenCalled()
        {
            // Arrange
            var changed = new TestNotifyPropertyChanged(m_Sut,
                                                        "ExceptionThrown");
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = "Text"
                          };

            // Act
            m_Sut.ExceptionThrownChangedHandler(message);

            // Assert
            Assert.True(changed.IsExpectedNotified);
        }

        [Test]
        public void ExceptionThrownChangedHandler_RaisesEventIsClearErrorEnabled_WhenCalled()
        {
            // Arrange
            var changed = new TestNotifyPropertyChanged(m_Sut,
                                                        "IsClearErrorEnabled");
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = "Text"
                          };

            // Act
            m_Sut.ExceptionThrownChangedHandler(message);

            // Assert
            Assert.True(changed.IsExpectedNotified);
        }

        [Test]
        public void ExceptionThrownChangedHandler_SetsExceptionThrownToEmpty_WhenTextIsNull()
        {
            // Arrange
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = null
                          };

            // Act
            m_Sut.ExceptionThrownChangedHandler(message);

            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.ExceptionThrown);
        }

        [Test]
        public void ExceptionThrownChangedHandler_SetsStatus_WhenCalled()
        {
            // Arrange
            const string expected = "Text";
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = expected
                          };

            // Act
            m_Sut.ExceptionThrownChangedHandler(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.ExceptionThrown);
        }

        [Test]
        public void IsClearErrorEnabled_ReturnsFalse_ForExceptionThrownIsEmpty()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Sut.IsClearErrorEnabled);
        }

        [Test]
        public void IsClearErrorEnabled_ReturnsTrue_ForExceptionThrownIsNotEmpty()
        {
            // Arrange
            var message = new ExceptionThrownChangedMessage
                          {
                              Text = "Text"
                          };

            m_Sut.ExceptionThrownChangedHandler(message);

            // Act
            // Assert
            Assert.True(m_Sut.IsClearErrorEnabled);
        }

        [Test]
        public void SendClearErrorMessage_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Sut.SendClearErrorMessage();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ExceptionThrownClearErrorMessage>());
        }

        [Test]
        public void Status_ReturnsEmpty_ByDefault()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.Status);
        }

        [Test]
        public void StatusChangedHandler_RaisesEvent_WhenCalled()
        {
            // Arrange
            var changed = new TestNotifyPropertyChanged(m_Sut,
                                                        "Status");
            var message = new StatusChangedMessage
                          {
                              Text = "Text"
                          };

            // Act
            m_Sut.StatusChangedHandler(message);

            // Assert
            Assert.True(changed.IsExpectedNotified);
        }

        [Test]
        public void StatusChangedHandler_SetsStatus_WhenCalled()
        {
            // Arrange
            const string expected = "Text";
            var message = new StatusChangedMessage
                          {
                              Text = expected
                          };

            // Act
            m_Sut.StatusChangedHandler(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.Status);
        }

        [Test]
        public void StatusChangedHandler_SetsStatusToEmpty_WhenTextIsNull()
        {
            // Arrange
            var message = new StatusChangedMessage
                          {
                              Text = null
                          };

            // Act
            m_Sut.StatusChangedHandler(message);

            // Assert
            Assert.AreEqual(string.Empty,
                            m_Sut.Status);
        }
    }
}