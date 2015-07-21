using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;

namespace Selkie.WPF.Models.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class ShortestPathDirectionModelTests
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

            m_Model = new ShortestPathDirectionModel(m_Logger,
                                                     m_Bus,
                                                     m_Helper);
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private INodeIdHelper m_Helper;
        private ShortestPathDirectionModel m_Model;
        private ILine m_Line;

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

        private ColonyBestTrailMessage CreateBestTrailMessage()
        {
            var message = new ColonyBestTrailMessage
                          {
                              Iteration = 1, // todo fix spelling iteration
                              Trail = new[]
                                      {
                                          0,
                                          1
                                      },
                              Length = 123.0,
                              Type = "Type",
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3
                          };

            return message;
        }

        [Test]
        public void BestTrailHandler_CallsUpdate_WhenCalled()
        {
            // Arrange
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            // Act
            m_Model.ColonyBestTrailHandler(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <ShortestPathDirectionModelChangedMessage>());
        }

        [Test]
        public void ColonyLinesChangedHandler_CallsUpdateNodes_WhenCalled()
        {
            // Arrange
            var message = new ColonyLinesChangedMessage();

            // Act
            m_Model.ColonyLinesChangedHandler(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <ShortestPathDirectionModelChangedMessage>());
        }

        [Test]
        public void Constructor_SubscribesToBestTrailMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyBestTrailMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribesToColonyLinesChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyLinesChangedMessage, Task>>());
        }

        [Test]
        public void CreateNodeModel_ReturnsModel_ForForwardNode()
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
        public void CreateNodeModel_ReturnsModel_ForReverseNode()
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
        public void CreateNodeModel_ReturnsUnknownModel_ForUnknownNode()
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
        public void Nodes_IsNotNull_WhenCalledFirstTime()
        {
            Assert.NotNull(m_Model.Nodes);
        }

        [Test]
        public void Update_CallsUpdateNodes_WhenCalled()
        {
            // Arrange
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            // Act
            m_Model.Update(message);

            // Assert
            Assert.AreEqual(2,
                            m_Model.Nodes.Count());
        }

        [Test]
        public void Update_SendsMessage_WhenCalled()
        {
            // Arrange
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            // Act
            m_Model.Update(message);

            // Assert
            m_Bus.Received().Publish(Arg.Any <ShortestPathDirectionModelChangedMessage>());
        }

        [Test]
        public void UpdateNodes_AddsNodeToNodes_ForTrail()
        {
            // Arrange
            var trail = new[]
                        {
                            2,
                            0
                        };

            // Act
            m_Model.UpdateNodes(trail);

            // Assert
            Assert.AreEqual(2,
                            m_Model.Nodes.Count());
        }

        [Test]
        public void UpdateNodes_ClearsNodes_ForEmptyTrail()
        {
            // Arrange

            // Act
            m_Model.UpdateNodes(new int[0]);

            // Assert
            Assert.AreEqual(0,
                            m_Model.Nodes.Count());
        }
    }
}