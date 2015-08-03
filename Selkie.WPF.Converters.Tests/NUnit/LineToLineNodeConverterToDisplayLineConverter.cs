using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Common;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LineToLineNodeConverterToDisplayLineConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_NodeOne = CreateNode();
            m_NodeTwo = CreateNode();

            m_Disposer = Substitute.For <IDisposer>();

            m_Factory = Substitute.For <IDisplayLineFactory>();

            m_Converter = new LineToLineNodeConverterToDisplayLineConverter(m_Disposer,
                                                                            m_Factory);
        }

        private LineToLineNodeConverterToDisplayLineConverter m_Converter;
        private IDisplayLineFactory m_Factory;
        private ILineToLineNodeConverter m_NodeOne;
        private ILineToLineNodeConverter m_NodeTwo;
        private IDisposer m_Disposer;

        private ILineToLineNodeConverter CreateNode()
        {
            var fromConverter = Substitute.For <ILine>();
            var toConverter = Substitute.For <ILine>();

            var node = Substitute.For <ILineToLineNodeConverter>();
            node.From.Returns(fromConverter);
            node.FromDirection.Returns(Constants.LineDirection.Forward);
            node.To.Returns(toConverter);
            node.ToDirection.Returns(Constants.LineDirection.Reverse);

            return node;
        }

        [Test]
        public void ConstructorAddsToDisposerTest()
        {
            m_Disposer.Received().AddResource(m_Converter.ReleaseDisplayLines);
        }

        [Test]
        public void ConvertersRoundtripTest()
        {
            var nodesOld = new[]
                           {
                               Substitute.For <ILineToLineNodeConverter>()
                           };

            var nodesNew = new[]
                           {
                               Substitute.For <ILineToLineNodeConverter>(),
                               Substitute.For <ILineToLineNodeConverter>()
                           };

            m_Converter.Converters = nodesOld;

            Assert.AreEqual(nodesOld,
                            m_Converter.Converters,
                            "Old");

            m_Converter.Converters = nodesNew;

            Assert.AreEqual(nodesNew,
                            m_Converter.Converters,
                            "New");
        }

        [Test]
        public void ConvertersSetTest()
        {
            var nodesOld = new[]
                           {
                               Substitute.For <ILineToLineNodeConverter>()
                           };

            m_Converter.Converters = nodesOld;

            Assert.AreEqual(nodesOld,
                            m_Converter.Converters);
        }

        [Test]
        public void CreateDisplayLinesForNodesCallsCreateCallsCreateForNodeTwoFromLineTest()
        {
            var nodes = new[]
                        {
                            m_NodeOne,
                            m_NodeTwo
                        };

            m_Converter.CreateDisplayLinesForNodes(nodes);

            m_Factory.Received().Create(m_NodeTwo.From,
                                        m_NodeTwo.FromDirection);
        }

        [Test]
        public void CreateDisplayLinesForNodesCallsCreateCallsCreateForNodeTwoToLineTest()
        {
            var nodes = new[]
                        {
                            m_NodeOne,
                            m_NodeTwo
                        };

            m_Converter.CreateDisplayLinesForNodes(nodes);

            m_Factory.Received().Create(m_NodeTwo.To,
                                        m_NodeTwo.ToDirection);
        }

        [Test]
        public void CreateDisplayLinesForNodesForTwoNodesCountTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>(),
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            List <IDisplayLine> actual = m_Converter.CreateDisplayLinesForNodes(nodes);

            Assert.AreEqual(3,
                            actual.Count);
        }

        [Test]
        public void CreateDisplayLinesForOneNodeCountTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            List <IDisplayLine> actual = m_Converter.CreateDisplayLines(nodes);

            Assert.AreEqual(2,
                            actual.Count);
        }

        [Test]
        public void CreateDisplayLinesForTwoNodesCountTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>(),
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            List <IDisplayLine> actual = m_Converter.CreateDisplayLines(nodes);

            Assert.AreEqual(3,
                            actual.Count);
        }

        [Test]
        public void CreateDisplayLinesSingleNodeCallsCreateForFromLineTest()
        {
            var nodes = new[]
                        {
                            m_NodeOne
                        };

            m_Converter.CreateDisplayLinesSingleNode(nodes);

            m_Factory.Received().Create(m_NodeOne.From,
                                        m_NodeOne.FromDirection);
        }

        [Test]
        public void CreateDisplayLinesSingleNodeCallsCreateForNodeOneFromLineOneTest()
        {
            var nodes = new[]
                        {
                            m_NodeOne,
                            m_NodeTwo
                        };

            m_Converter.CreateDisplayLinesForNodes(nodes);

            m_Factory.Received().Create(m_NodeOne.From,
                                        m_NodeOne.FromDirection);
        }

        [Test]
        public void CreateDisplayLinesSingleNodeCallsCreateForToLineTest()
        {
            var nodes = new[]
                        {
                            m_NodeOne
                        };

            m_Converter.CreateDisplayLinesSingleNode(nodes);

            m_Factory.Received().Create(m_NodeOne.To,
                                        m_NodeOne.ToDirection);
        }

        [Test]
        public void CreateDisplayLinesSingleNodeCountTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            List <IDisplayLine> actual = m_Converter.CreateDisplayLinesSingleNode(nodes);

            Assert.AreEqual(2,
                            actual.Count);
        }

        [Test]
        public void DefaultConvertersTest()
        {
            Assert.NotNull(m_Converter.Converters);
        }

        [Test]
        public void DefaultDisplayLinesTest()
        {
            Assert.NotNull(m_Converter.DisplayLines);
        }

        [Test]
        public void DisposeTest()
        {
            var converter = new LineToLineNodeConverterToDisplayLineConverter(m_Disposer,
                                                                              m_Factory);

            converter.Dispose();

            m_Disposer.Received().Dispose();
        }

        [Test]
        public void ReleaseDisplayLinesCallsClearsListTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            m_Converter.Converters = nodes;
            m_Converter.Convert();

            m_Converter.ReleaseDisplayLines();

            Assert.AreEqual(0,
                            m_Converter.DisplayLines.Count());
        }

        [Test]
        public void ReleaseDisplayLinesCallsReleaseForTwoNodesTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>(),
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            m_Converter.Converters = nodes;
            m_Converter.Convert();

            m_Converter.ReleaseDisplayLines();

            m_Factory.Received(3).Release(Arg.Any <IDisplayLine>());
        }

        [Test]
        public void ReleaseDisplayLinesCallsReleaseTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };

            m_Converter.Converters = nodes;
            m_Converter.Convert();

            m_Converter.ReleaseDisplayLines();

            m_Factory.Received(2).Release(Arg.Any <IDisplayLine>());
        }
    }
}