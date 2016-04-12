using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Status;
using Selkie.WPF.ViewModels.Tests.NUnit;

namespace Selkie.WPF.ViewModels.Tests.Status.NUnit
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
            m_Model = Substitute.For <IStatusModel>();

            m_Sut = new StatusViewModel(m_Bus,
                                        m_Dispatcher,
                                        m_Model);
        }

        private ISelkieInMemoryBus m_Bus;
        private IApplicationDispatcher m_Dispatcher;
        private IStatusModel m_Model;
        private StatusViewModel m_Sut;

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