using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Tests.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class NodeIdHelperTests
    {
        [SetUp]
        public void Setup()
        {
            m_Lines = CreateLines();

            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Manager.Lines.Returns(m_Lines);

            m_Sut = new NodeIdHelper(m_Manager);
        }

        private ILinesSourceManager m_Manager;
        private IEnumerable <ILine> m_Lines;
        private NodeIdHelper m_Sut;

        private IEnumerable <Line> CreateLines()
        {
            var line1StartPoint = new Point(30.0,
                                            0.0);
            var line1EndPoint = new Point(40.0,
                                          0.0);
            var line1 = new Line(0,
                                 line1StartPoint,
                                 line1EndPoint);

            var line2StartPoint = new Point(0.0,
                                            40.0);
            var line2EndPoint = new Point(60.0,
                                          40.0);
            var line2 = new Line(1,
                                 line2StartPoint,
                                 line2EndPoint);

            var line3StartPoint = new Point(-30.0,
                                            80.0);
            var line3EndPoint = new Point(90.0,
                                          80.0);
            var line3 = new Line(2,
                                 line3StartPoint,
                                 line3EndPoint);

            var line4StartPoint = new Point(-30.0,
                                            -80.0);
            var line4EndPoint = new Point(90.0,
                                          -80.0);
            var line4 = new Line(3,
                                 line4StartPoint,
                                 line4EndPoint);

            return new List <Line>
                   {
                       line1,
                       line2,
                       line3,
                       line4
                   };
        }

        [Test]
        public void GetLine_ReturnsLine_ForKnownId()
        {
            // Arrange
            ILine expected = m_Lines.First(x => x.Id == 1);

            // Act
            ILine actual = m_Sut.GetLine(1);

            // Assert
            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void GetLine_ReturnsNull_ForUnknownId()
        {
            // Arrange
            // Act
            ILine actual = m_Sut.GetLine(-123);

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void IsForwardNode_ReturnsFalse_ForNodeIdThree()
        {
            Assert.False(m_Sut.IsForwardNode(3));
        }

        [Test]
        public void IsForwardNode_ReturnsTrue_ForNodeIdTwo()
        {
            Assert.True(m_Sut.IsForwardNode(2));
        }

        [Test]
        public void IsForwardNode_ReturnsTrue_ForNodeIdZero()
        {
            Assert.True(m_Sut.IsForwardNode(0));
        }

        [Test]
        public void IsForwardNodeReturnsFalseForNodeIdOne()
        {
            Assert.False(m_Sut.IsForwardNode(1));
        }

        [Test]
        public void NodeToLine_ReturnsZero_ForNodeIdOne()
        {
            Assert.AreEqual(0,
                            m_Sut.NodeToLine(1));
        }

        [Test]
        public void NodeToLine_ReturnsZero_ForNodeIdThree()
        {
            Assert.AreEqual(1,
                            m_Sut.NodeToLine(3));
        }

        [Test]
        public void NodeToLine_ReturnsZero_ForNodeIdTwo()
        {
            Assert.AreEqual(1,
                            m_Sut.NodeToLine(2));
        }

        [Test]
        public void NodeToLine_ReturnsZero_ForNodeIdZero()
        {
            Assert.AreEqual(0,
                            m_Sut.NodeToLine(0));
        }

        [Test]
        public void Reverse_ReturnsOne_ForNodeIdZero()
        {
            Assert.AreEqual(1,
                            m_Sut.Reverse(0));
        }

        [Test]
        public void Reverse_ReturnsThree_ForNodeIdTwo()
        {
            Assert.AreEqual(3,
                            m_Sut.Reverse(2));
        }

        [Test]
        public void Reverse_ReturnsTwo_ForNodeIdThree()
        {
            Assert.AreEqual(2,
                            m_Sut.Reverse(3));
        }

        [Test]
        public void Reverse_ReturnsZero_ForNodeIdOne()
        {
            Assert.AreEqual(0,
                            m_Sut.Reverse(1));
        }
    }
}