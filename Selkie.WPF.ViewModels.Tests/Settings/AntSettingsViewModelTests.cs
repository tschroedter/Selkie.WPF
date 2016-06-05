using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.Settings;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Settings;

namespace Selkie.WPF.ViewModels.Tests.Settings
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class AntSettingsViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_Manager = Substitute.For <ICommandManager>();

            m_Sut = new AntSettingsViewModel(m_Logger,
                                             m_Bus,
                                             m_Dispatcher,
                                             m_Manager,
                                             Substitute.For <IAntSettingsModel>());

            m_SelectedNode = Substitute.For <IAntSettingsNode>();
            m_Sut.SelectedNode = m_SelectedNode;
        }

        private ISelkieLogger m_Logger;
        private ISelkieInMemoryBus m_Bus;
        private TestImmediateDispatcher m_Dispatcher;
        private ICommandManager m_Manager;
        private AntSettingsViewModel m_Sut;
        private IAntSettingsNode m_SelectedNode;

        private IEnumerable <IAntSettingsNode> CreatedTestNodes()
        {
            var one = Substitute.For <IAntSettingsNode>();
            one.Id.Returns(0);

            var two = Substitute.For <IAntSettingsNode>();
            two.Id.Returns(1);

            var nodes = new[]
                        {
                            one,
                            two
                        };

            return nodes;
        }

        private AntSettingsModelChangedMessage CreateAntSettingsModelChangedMessage()
        {
            var message = new AntSettingsModelChangedMessage(true,
                                                             1,
                                                             CreatedTestNodes());

            return message;
        }

        [Test]
        public void AntSettingsModelChangedHandler_CallsDispatcher_WhenCalled()
        {
            // Arrange
            AntSettingsModelChangedMessage message = CreateAntSettingsModelChangedMessage();

            var dispatcher = Substitute.For <IApplicationDispatcher>();

            var sut = new AntSettingsViewModel(m_Logger,
                                               m_Bus,
                                               dispatcher,
                                               m_Manager,
                                               Substitute.For <IAntSettingsModel>());

            // Act
            sut.AntSettingsModelChangedHandler(message);

            // Assert
            dispatcher.Received()
                      .BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void Apply_RaisesEventIsApplyEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "IsApplyEnabled");
            // Act
            m_Sut.Apply();

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "IsApplyEnabled");
        }

        [Test]
        public void Apply_SendsMessage_ForSelectedNodeIsNull()
        {
            // Arrange
            m_Sut.SelectedNode = null;

            // Act
            m_Sut.Apply();

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <AntSettingsModelSetMessage>(x =>
                                                                   x.IsFixedStartNode == m_Sut.IsFixedStartNode &&
                                                                   x.FixedStartNode == 0));
        }

        [Test]
        public void Apply_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            m_Sut.Apply();

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <AntSettingsModelSetMessage>(x =>
                                                                   x.IsFixedStartNode == m_Sut.IsFixedStartNode &&
                                                                   x.FixedStartNode == m_Sut.SelectedNode.Id));
        }

        [Test]
        public void Apply_SendsMessageWidthDefaultStartNode_ForSelectedNodeIsNull()
        {
            // Arrange
            m_Sut.SelectedNode = null;

            // Act
            m_Sut.Apply();

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <AntSettingsModelSetMessage>(x =>
                                                                   x.IsFixedStartNode == m_Sut.IsFixedStartNode &&
                                                                   x.FixedStartNode == 0));
        }

        [Test]
        public void Apply_SetsIsApplyingToTrue_WhenCalled()
        {
            // Arrange
            // Act
            m_Sut.Apply();

            // Assert
            Assert.True(m_Sut.IsApplying);
        }

        [Test]
        public void ApplyCommand_ReturnsCommand_WhenCalled()
        {
            // Arrange
            // Act
            ICommand actual = m_Sut.ApplyCommand;

            // Assert
            Assert.NotNull(actual);
        }

        [Test]
        public void ApplyCommandCanExecute_ReturnsFalse_ForIsApplyingIsTrue()
        {
            // Arrange
            m_Sut.Apply();

            // Act
            bool actual = m_Sut.ApplyCommandCanExecute();

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void ApplyCommandCanExecute_ReturnsTrue_ForIsApplyingIsFalse()
        {
            // Arrange
            // Act
            bool actual = m_Sut.ApplyCommandCanExecute();

            // Assert
            Assert.True(actual);
        }

        [Test]
        public void Constructor_SendsRequestMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <AntSettingsModelRequestMessage>());
        }

        [Test]
        public void Constructor_SubscribesToRacetrackSettingsSetMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .SubscribeAsync(m_Sut.GetType().ToString(),
                                 Arg.Any <Action <AntSettingsModelChangedMessage>>());
        }

        [Test]
        public void IsApplyEnabled_RetursDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.True(m_Sut.IsApplyEnabled);
        }

        [Test]
        public void IsFixedStartNode_RetursDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Sut.IsFixedStartNode);
        }

        [Test]
        public void Nodes_RaisesEvent_ForNewValue()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "Nodes");

            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "Nodes");
        }

        [Test]
        public void Nodes_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0,
                            m_Sut.Nodes.Count());
        }

        [Test]
        public void Nodes_SetsNewValue_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> expected = CreatedTestNodes();

            // Act
            m_Sut.Update(true,
                         1,
                         expected);


            // Assert
            Assert.AreEqual(expected,
                            m_Sut.Nodes);
        }

        [Test]
        public void SelectedNode_RaisesEvent_ForNewValue()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "SelectedNode");

            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "SelectedNode");
        }

        [Test]
        public void SelectedNode_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(m_Sut.SelectedNode);
        }

        [Test]
        public void SelectedNode_SetsNewValue_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> nodes = CreatedTestNodes().ToArray();
            IAntSettingsNode expected = nodes.First(x => x.Id == 1);


            // Act
            m_Sut.Update(true,
                         1,
                         nodes);


            // Assert
            Assert.AreEqual(expected,
                            m_Sut.SelectedNode);
        }

        [Test]
        public void Update_RaisesEventFixedStartNode_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "SelectedNode");
            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "SelectedNode");
        }

        [Test]
        public void Update_RaisesEventIsFixedStartNode_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "IsFixedStartNode");
            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "IsFixedStartNode");
        }

        [Test]
        public void Update_RaisesEventNodes_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "Nodes");
            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "Nodes");
        }

        [Test]
        public void Update_SetsIsFixedStartNode_WhenCalled()
        {
            // Arrange
            // Act
            m_Sut.Update(true,
                         1,
                         CreatedTestNodes());

            // Assert
            Assert.True(m_Sut.IsFixedStartNode);
        }

        [Test]
        public void Update_SetsNodes_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> expected = CreatedTestNodes();

            // Act
            m_Sut.Update(true,
                         1,
                         expected);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.Nodes);
        }

        [Test]
        public void Update_SetsSelectedNode_WhenCalled()
        {
            // Arrange
            // Act
            var expected = new AntSettingsNode(1,
                                               "1");

            m_Sut.Update(true,
                         1,
                         new IAntSettingsNode[]
                         {
                             expected
                         });

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.SelectedNode);
        }

        [Test]
        public void Update_SetsSelectedNode_WhenSelectedNodeDoesNotMatch()
        {
            // Arrange
            // Act
            var zero = new AntSettingsNode(0,
                                           "0");
            var one = new AntSettingsNode(1,
                                          "1");

            m_Sut.Update(true,
                         123,
                         new IAntSettingsNode[]
                         {
                             zero,
                             one
                         });

            // Assert
            Assert.AreEqual(zero,
                            m_Sut.SelectedNode);
        }

        [Test]
        public void UpdateAndNotify_CallsUpdate_WhenCalled()
        {
            // Arrange
            AntSettingsModelChangedMessage message = CreateAntSettingsModelChangedMessage();

            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "IsFixedStartNode");
            // Act
            m_Sut.UpdateAndNotify(message);

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "Did not call method Update(...)");
        }

        [Test]
        public void UpdateAndNotify_RaisesEventIsApplyEnabled_WhenCalled()
        {
            // Arrange
            AntSettingsModelChangedMessage message = CreateAntSettingsModelChangedMessage();

            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "IsApplyEnabled");
            // Act
            m_Sut.UpdateAndNotify(message);

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "IsApplyEnabled");
        }

        [Test]
        public void UpdateAndNotify_SetsIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            AntSettingsModelChangedMessage message = CreateAntSettingsModelChangedMessage();

            // Act
            m_Sut.UpdateAndNotify(message);

            // Assert
            Assert.False(m_Sut.IsApplying,
                         "IsApplying");
        }
    }
}