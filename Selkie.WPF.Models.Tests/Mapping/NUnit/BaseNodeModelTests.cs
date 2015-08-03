using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class BaseNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Line = CreateLine();

            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Helper = Substitute.For <INodeIdHelper>();
            m_Helper.GetLine(-1).ReturnsForAnyArgs(m_Line);
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(true);

            m_Model = new TestBaseNodeModel(m_Logger,
                                            m_Bus,
                                            m_Helper);
        }

        private IBus m_Bus;
        private ILogger m_Logger;
        private TestBaseNodeModel m_Model;
        private INodeIdHelper m_Helper;
        private ILine m_Line;

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
            public TestBaseNodeModel(ILogger logger,
                                     IBus bus,
                                     INodeIdHelper nodeIdHelper)
                : base(logger,
                       bus,
                       nodeIdHelper)
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
        public void ColonyLinesChangedHandlerCallsUpdateTest()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Model.ColonyLinesChangedHandler(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <TestBaseNodeModelChangedMessage>());
        }

        [Test]
        public void ColonyLinesChangedHandlerClearsNodeModelTest()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Model.ColonyLinesChangedHandler(message);

            // Assert
            Assert.True(m_Model.Node == NodeModel.Unknown);
        }

        [Test]
        public void CreateNodeModelReturnsModelForForwardNodeTest()
        {
            // Arrange
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(true);

            // Act
            INodeModel actual = m_Model.CreateNodeModel(1,
                                                        2);

            // Assert
            Assert.AreEqual(2,
                            actual.Id,
                            "Id");
            Assert.AreEqual(m_Line.X1,
                            actual.X,
                            "X");
            Assert.AreEqual(m_Line.Y1,
                            actual.Y,
                            "Y");
            Assert.AreEqual(Angle.For45Degrees,
                            actual.DirectionAngle,
                            "DirectionAngle");
        }

        [Test]
        public void CreateNodeModelReturnsModelForReverseNodeTest()
        {
            // Arrange
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(false);

            // Act
            INodeModel actual = m_Model.CreateNodeModel(1,
                                                        3);

            // Assert
            Assert.AreEqual(3,
                            actual.Id,
                            "Id");
            Assert.AreEqual(m_Line.X2,
                            actual.X,
                            "X");
            Assert.AreEqual(m_Line.Y2,
                            actual.Y,
                            "Y");
            Assert.AreEqual(Angle.For225Degrees,
                            actual.DirectionAngle,
                            "DirectionAngle");
        }

        [Test]
        public void CreateNodeModelReturnsUnknownModelForUnknownNodeTest()
        {
            // Arrange
            m_Helper.GetLine(-1).ReturnsForAnyArgs(( ILine ) null);
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(false);

            // Act
            INodeModel actual = m_Model.CreateNodeModel(-123,
                                                        3);

            // Assert
            Assert.AreEqual(NodeModel.Unknown,
                            actual);
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
                                            Arg.Any <Func <ColonyBestTrailMessage, Task>>());
        }

        [Test]
        public void SubscribeToColonyLinesChangedMessageest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyLinesChangedMessage, Task>>());
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
            INodeModel actual = m_Model.Node;

            Assert.AreEqual(2,
                            actual.Id,
                            "Id");
            Assert.AreEqual(line.X1,
                            actual.X,
                            "X");
            Assert.AreEqual(line.Y1,
                            actual.Y,
                            "Y");
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