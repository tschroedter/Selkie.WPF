using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodeModelCreatorTests
    {
        [SetUp]
        public void Setup()
        {
            m_Line = CreateLine();

            m_Helper = Substitute.For <INodeIdHelper>();
            m_Helper.GetLine(-1).ReturnsForAnyArgs(m_Line);
            m_Helper.IsForwardNode(-1).ReturnsForAnyArgs(true);

            m_Model = new NodeModelCreator(m_Helper);
        }

        private NodeModelCreator m_Model;
        private INodeIdHelper m_Helper;
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
    }
}