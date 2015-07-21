using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Selkie.Common;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodeIndexHelperTests
    {
        [SetUp]
        public void Setup()
        {
            m_LineOne = new Line(0,
                                 0.0,
                                 10.0,
                                 100.0,
                                 10.0);
            m_LineTwo = new Line(0,
                                 0.0,
                                 50.0,
                                 100.0,
                                 50.0);

            m_NodeIndexToLineConverter = Substitute.For <INodeIndexToLineConverter>();
            m_NodeIndexToLineConverter.Line.Returns(LineForIndex);

            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Manager.Lines.Returns(new[]
                                    {
                                        m_LineOne,
                                        m_LineTwo
                                    });

            m_Helper = new NodeIndexHelper(m_Manager,
                                           m_NodeIndexToLineConverter);
        }

        private Line m_LineOne;
        private Line m_LineTwo;
        private INodeIndexToLineConverter m_NodeIndexToLineConverter;
        private NodeIndexHelper m_Helper;
        private ILinesSourceManager m_Manager;

        private ILine LineForIndex(CallInfo callInfo)
        {
            switch ( m_NodeIndexToLineConverter.NodeIndex )
            {
                case 0:
                case 1:
                    return m_LineOne;

                default:
                    return m_LineTwo;
            }
        }

        [Test]
        public void IsValidIndexReturnsFalseForBeyondEndTest()
        {
            Assert.False(m_Helper.IsValidIndex(-1000));
        }

        [Test]
        public void IsValidIndexReturnsFalseForNegativeTest()
        {
            Assert.False(m_Helper.IsValidIndex(-1));
        }

        [Test]
        public void IsValidIndexReturnsTrueForValidIndexTest()
        {
            Assert.True(m_Helper.IsValidIndex(0));
        }

        [Test]
        public void NodeIndexToDirectionReturnsForwardForEvenIndexTest()
        {
            Assert.AreEqual(Constants.LineDirection.Forward,
                            m_Helper.NodeIndexToDirection(0),
                            "0");
            Assert.AreEqual(Constants.LineDirection.Forward,
                            m_Helper.NodeIndexToDirection(2),
                            "2");
        }

        [Test]
        public void NodeIndexToDirectionReturnsForwardForOddIndexTest()
        {
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            m_Helper.NodeIndexToDirection(1),
                            "1");
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            m_Helper.NodeIndexToDirection(3),
                            "3");
        }

        [Test]
        public void NodeIndexToDirectionThrowsForUnknownIndexTest()
        {
            Assert.Throws <ArgumentException>(() => m_Helper.NodeIndexToDirection(-1),
                                              "-1");
            Assert.Throws <ArgumentException>(() => m_Helper.NodeIndexToDirection(1000),
                                              "100");
        }

        [Test]
        public void NodeIndexToLineTest()
        {
            Assert.AreEqual(m_LineOne,
                            m_Helper.NodeIndexToLine(1));
        }
    }
}