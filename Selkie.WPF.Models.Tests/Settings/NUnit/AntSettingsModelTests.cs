using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.Settings;

namespace Selkie.WPF.Models.Tests.Settings.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class AntSettingsModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Manager = Substitute.For <IAntSettingsNodesManager>();

            m_Sut = new AntSettingsModel(m_Bus,
                                         m_Manager);
        }

        private ISelkieInMemoryBus m_Bus;
        private AntSettingsModel m_Sut;
        private IAntSettingsNodesManager m_Manager;

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

        [Test]
        public void AntSettingsModelRequestHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new AntSettingsModelRequestMessage();

            // Act
            m_Sut.AntSettingsModelRequestHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <AntSettingsModelChangedMessage>(x =>
                                                                       x.IsFixedStartNode ==
                                                                       m_Sut.IsFixedStartNode &&
                                                                       x.FixedStartNode ==
                                                                       m_Sut.FixedStartNode &&
                                                                       x.Nodes.SequenceEqual(m_Manager.Nodes)));
        }

        [Test]
        public void AntSettingsModelSetHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new AntSettingsModelSetMessage
                          {
                              IsFixedStartNode = true,
                              FixedStartNode = 123
                          };

            // Act
            m_Sut.AntSettingsModelSetHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <ColonyAntSettingsSetMessage>(x =>
                                                                    x.IsFixedStartNode ==
                                                                    message.IsFixedStartNode &&
                                                                    x.FixedStartNode == message.FixedStartNode));
        }

        [Test]
        public void ColonyAntSettingsResponseHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyAntSettingsResponseMessage
                          {
                              IsFixedStartNode = true,
                              FixedStartNode = 123
                          };

            // Act
            m_Sut.ColonyAntSettingsResponseHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Is <AntSettingsModelChangedMessage>(x =>
                                                                       x.IsFixedStartNode ==
                                                                       message.IsFixedStartNode &&
                                                                       x.FixedStartNode ==
                                                                       message.FixedStartNode &&
                                                                       x.Nodes.SequenceEqual(m_Manager.Nodes)));
        }

        [Test]
        public void ColonyAntSettingsResponseHandler_SetsFixedStartNode_WhenCalled()
        {
            // Arrange
            var message = new ColonyAntSettingsResponseMessage
                          {
                              FixedStartNode = 123
                          };

            // Act
            m_Sut.ColonyAntSettingsResponseHandler(message);

            // Assert
            Assert.AreEqual(123,
                            m_Sut.FixedStartNode);
        }

        [Test]
        public void ColonyAntSettingsResponseHandler_SetsIsFixedStartNode_WhenCalled()
        {
            // Arrange
            var message = new ColonyAntSettingsResponseMessage
                          {
                              IsFixedStartNode = true
                          };

            // Act
            m_Sut.ColonyAntSettingsResponseHandler(message);

            // Assert
            Assert.True(m_Sut.IsFixedStartNode);
        }

        [Test]
        public void ColonyLinesChangedHandler_CallsManager_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Sut.ColonyLinesChangedHandler(message);

            // Assert
            m_Manager.Received().CreateNodesForCurrentLines();
        }

        [Test]
        public void ColonyLinesChangedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Sut.ColonyLinesChangedHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <AntSettingsModelChangedMessage>());
        }

        [Test]
        public void Constructor_SendsMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .PublishAsync(Arg.Any <ColonyLinesRequestMessage>());
        }

        [Test]
        public void Constructor_SubscribesToAntSettingsModelRequestMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .SubscribeAsync(m_Sut.GetType().ToString(),
                                 Arg.Any <Action <AntSettingsModelRequestMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToAntSettingsModelSetMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .SubscribeAsync(m_Sut.GetType().ToString(),
                                 Arg.Any <Action <AntSettingsModelSetMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToColonyAntSettingsResponseMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .SubscribeAsync(m_Sut.GetType().ToString(),
                                 Arg.Any <Action <ColonyAntSettingsResponseMessage>>());
        }

        [Test]
        public void Constructor_SubscribesToColonyLinesChangedMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received()
                 .SubscribeAsync(m_Sut.GetType().ToString(),
                                 Arg.Any <Action <ColonyLinesChangedMessage>>());
        }

        [Test]
        public void Nodes_SendsMessage_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> nodes = CreatedTestNodes();
            m_Manager.Nodes.Returns(nodes);

            // Act
            // Assert
            Assert.AreEqual(nodes,
                            m_Sut.Nodes);
        }
    }
}