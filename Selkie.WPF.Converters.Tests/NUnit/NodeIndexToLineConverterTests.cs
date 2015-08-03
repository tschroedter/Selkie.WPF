using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodeIndexToLineConverterTests
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
            m_Lines = new[]
                      {
                          m_LineOne,
                          m_LineTwo
                      };

            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Manager.Lines.Returns(m_Lines);

            m_Converter = new NodeIndexToLineConverter(m_Manager);
        }

        private NodeIndexToLineConverter m_Converter;
        private Line m_LineOne;
        private Line m_LineTwo;
        private Line[] m_Lines;
        private ILinesSourceManager m_Manager;

        [Test]
        public void ConvertTest()
        {
            m_Converter.NodeIndex = 3;

            m_Converter.Convert();

            Assert.AreEqual(m_LineTwo,
                            m_Converter.Line);
        }

        [Test]
        public void GetLineByNodeIndexForIndexBehindLengthTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(1000);

            Assert.AreEqual(Line.Unknown,
                            actual);
        }

        [Test]
        public void GetLineByNodeIndexForIndexBelowZeroTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(-100);

            Assert.AreEqual(Line.Unknown,
                            actual);
        }

        [Test]
        public void GetLineByNodeIndexForLineOneForwardTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(0);

            Assert.AreEqual(m_LineOne,
                            actual);
        }

        [Test]
        public void GetLineByNodeIndexForLineOneReverseTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(1);

            Assert.AreEqual(m_LineOne,
                            actual);
        }

        [Test]
        public void GetLineByNodeIndexForLineTwoForwardTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(2);

            Assert.AreEqual(m_LineTwo,
                            actual);
        }

        [Test]
        public void GetLineByNodeIndexForLineTwoReverseTest()
        {
            ILine actual = m_Converter.GetLineByNodeIndex(3);

            Assert.AreEqual(m_LineTwo,
                            actual);
        }

        [Test]
        public void LineDefaultTest()
        {
            Assert.AreEqual(Line.Unknown,
                            m_Converter.Line);
        }

        [Test]
        public void NodeIndexDefaultTest()
        {
            Assert.AreEqual(int.MinValue,
                            m_Converter.NodeIndex);
        }

        [Test]
        public void NodeIndexRoundtripTest()
        {
            m_Converter.NodeIndex = 100;

            Assert.AreEqual(100,
                            m_Converter.NodeIndex);
        }
    }
}