using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class BaseNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Line = CreateLine();

            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Helper = Substitute.For <INodeIdHelper>();
            m_NodeModel = Substitute.For <INodeModel>();
            m_Creator = Substitute.For <INodeModelCreator>();
            m_Creator.CreateNodeModel(0,
                                      0).ReturnsForAnyArgs(m_NodeModel);
            m_Creator.Helper.Returns(m_Helper);
            m_Helper.GetLine(-1).ReturnsForAnyArgs(m_Line);
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(true);

            m_Model = new TestBaseNodeModel(m_Bus,
                                            m_Creator);
        }

        private INodeModel m_NodeModel;
        private INodeModelCreator m_Creator;
        private TestBaseNodeModel m_Model;
        private INodeIdHelper m_Helper;
        private ILine m_Line;
        private ISelkieInMemoryBus m_Bus;

        private ColonyBestTrailMessage CreateBestTrailMessage([NotNull] IEnumerable <int> trail)
        {
            var message = new ColonyBestTrailMessage
                          {
                              Iteration = 1,
                              Trail = trail,
                              Length = 123,
                              Type = "Type",
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3
                          };

            return message;
        }

        private ILine CreateLine()
        {
            var line = Substitute.For <ILine>();

            line.Id.Returns(1);
            line.X1.Returns(1.0);
            line.Y1.Returns(2.0);
            line.X2.Returns(3.0);
            line.Y2.Returns(4.0);
            line.AngleToXAxis.Returns(Angle.For45Degrees);

            return line;
        }

        private class TestBaseNodeModel : BaseNodeModel
        {
            public TestBaseNodeModel([NotNull] ISelkieInMemoryBus bus,
                                     [NotNull] INodeModelCreator nodeModelCreator)
                : base(bus,
                       nodeModelCreator)
            {
            }

            public override int DetermineNodeId(IEnumerable <int> trail)
            {
                return trail.First();
            }

            public override void SendMessage()
            {
                Bus.Publish(new TestBaseNodeModelChangedMessage());
            }
        }

        private class TestBaseNodeModelChangedMessage
        {
        }

        [Test]
        public void BestTrailHandlerCallsUpdateTest()
        {
            // Arrange
            ColonyBestTrailMessage message = CreateBestTrailMessage(new[]
                                                                    {
                                                                        2,
                                                                        0,
                                                                        4,
                                                                        6
                                                                    });

            // Act
            m_Model.ColonyBestTrailHandler(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <TestBaseNodeModelChangedMessage>());
        }

        [Test]
        public void ColonyLineResponsedHandlerCallsUpdateTest()
        {
            // Arrange
            var message = new ColonyLineResponseMessage();

            // Act
            m_Model.ColonyLineResponsedHandler(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <TestBaseNodeModelChangedMessage>());
        }

        [Test]
        public void ColonyLineResponsedHandlerClearsNodeModelTest()
        {
            // Arrange
            var message = new ColonyLineResponseMessage();

            // Act
            m_Model.ColonyLineResponsedHandler(message);

            // Assert
            Assert.True(m_Model.Node == NodeModel.Unknown);
        }

        [Test]
        public void DefaultNodeTest()
        {
            Assert.NotNull(m_Model.Node);
        }

        [Test]
        public void SubscribeToBestTrailMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ColonyBestTrailMessage>>());
        }

        [Test]
        public void SubscribeToColonyLinesResponseMessageest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ColonyLineResponseMessage>>());
        }

        [Test]
        public void UpdateCreatesNewNodesModelTest()
        {
            // Arrange
            ILine line = CreateLine();

            m_Helper.GetLine(-1).ReturnsForAnyArgs(line);
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(true);

            var trail = new[]
                        {
                            2,
                            0,
                            4,
                            6
                        };

            // Act
            m_Model.Update(trail);

            // Assert
            Assert.AreEqual(m_NodeModel,
                            m_Model.Node);
        }

        [Test]
        public void UpdateSendsMessageTest()
        {
            // Arrange
            var trail = new[]
                        {
                            2,
                            0,
                            4,
                            6
                        };

            // Act
            m_Model.Update(trail);

            // Assert
            m_Bus.Received().Publish(Arg.Any <TestBaseNodeModelChangedMessage>());
        }
    }
}