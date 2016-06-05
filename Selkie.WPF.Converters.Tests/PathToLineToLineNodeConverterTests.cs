using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Selkie.Common.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.NUnit.Extensions;
using Selkie.WPF.Converters.Interfaces;
using Constants = Selkie.Common.Constants;

namespace Selkie.WPF.Converters.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PathToLineToLineNodeConverterTests
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
            m_LineThree = new Line(0,
                                   0.0,
                                   70.0,
                                   100.0,
                                   70.0);

            m_NodeIndexHelper = Substitute.For <INodeIndexHelper>();
            m_NodeIndexHelper.NodeIndexToLine(0).ReturnsForAnyArgs(LineForIndex);
            m_NodeIndexHelper.NodeIndexToDirection(0).ReturnsForAnyArgs(DirectionForIndex);

            m_NodeConverterOne = Substitute.For <ILineToLineNodeConverter>();
            m_NodeConverterTwo = Substitute.For <ILineToLineNodeConverter>();

            m_Disposer = Substitute.For <IDisposer>();
            m_Factory = Substitute.For <IConverterFactory>();
            m_Factory.Create <ILineToLineNodeConverter>().Returns(m_NodeConverterOne,
                                                                  m_NodeConverterTwo);

            m_Converter = new PathToLineToLineNodeConverter(m_Disposer,
                                                            m_Factory,
                                                            m_NodeIndexHelper);
        }

        [TearDown]
        public void Teardown()
        {
            m_Converter.Dispose();
        }

        private ILine LineForIndex(CallInfo callInfo)
        {
            var index = ( int ) callInfo [ 0 ];

            switch ( index )
            {
                case 0:
                case 1:
                    return m_LineOne;

                case 2:
                case 3:
                    return m_LineTwo;

                default:
                    return m_LineThree;
            }
        }

        private Constants.LineDirection DirectionForIndex(CallInfo callInfo)
        {
            var index = ( int ) callInfo [ 0 ];

            switch ( index )
            {
                case 0:
                case 2:
                case 4:
                    return Constants.LineDirection.Forward;

                default:
                    return Constants.LineDirection.Reverse;
            }
        }

        private PathToLineToLineNodeConverter m_Converter;
        private INodeIndexHelper m_NodeIndexHelper;
        private ILine m_LineOne;
        private ILine m_LineTwo;
        private Line m_LineThree;
        private IConverterFactory m_Factory;
        private ILineToLineNodeConverter m_NodeConverterOne;
        private ILineToLineNodeConverter m_NodeConverterTwo;
        private IDisposer m_Disposer;

        [Test]
        public void ConstructorAddsToDisposerTest()
        {
            m_Disposer.Received().AddResource(m_Converter.ReleaseConverters);
        }

        [Test]
        public void ConvertForOneNodeTest()
        {
            m_Converter.Path = new[]
                               {
                                   0,
                                   3
                               };
            m_Converter.Convert();

            ILineToLineNodeConverter actual = m_Converter.Nodes.First();

            Assert.AreEqual(m_LineOne,
                            actual.From,
                            "Line");
            Assert.AreEqual(Constants.LineDirection.Forward,
                            actual.FromDirection,
                            "FromDirection");
            Assert.AreEqual(m_LineTwo,
                            actual.To,
                            "To");
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            actual.ToDirection,
                            "ToDirection");
        }

        [Test]
        public void ConvertForTwoNodesFirstTest()
        {
            m_Converter.Path = new[]
                               {
                                   0,
                                   3,
                                   4
                               };
            m_Converter.Convert();

            ILineToLineNodeConverter actual = m_Converter.Nodes.ElementAt(0);

            Assert.AreEqual(m_LineOne,
                            actual.From,
                            "Line");
            Assert.AreEqual(Constants.LineDirection.Forward,
                            actual.FromDirection,
                            "FromDirection");
            Assert.AreEqual(m_LineTwo,
                            actual.To,
                            "To");
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            actual.ToDirection,
                            "ToDirection");
        }

        [Test]
        public void ConvertForTwoNodesSecondTest()
        {
            m_Converter.Path = new[]
                               {
                                   0,
                                   3,
                                   4
                               };
            m_Converter.Convert();

            ILineToLineNodeConverter actual = m_Converter.Nodes.ElementAt(1);

            Assert.AreEqual(m_LineTwo,
                            actual.From,
                            "Line");
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            actual.FromDirection,
                            "FromDirection");
            Assert.AreEqual(m_LineThree,
                            actual.To,
                            "To");
            Assert.AreEqual(Constants.LineDirection.Forward,
                            actual.ToDirection,
                            "ToDirection");
        }

        [Test]
        public void ConvertTest()
        {
            m_Converter.Path = new[]
                               {
                                   0,
                                   3
                               };
            m_Converter.Convert();

            Assert.AreEqual(1,
                            m_Converter.Nodes.Count());
        }

        [Test]
        public void CreateLineToLineNodeTest()
        {
            ILineToLineNodeConverter actual = m_Converter.CreateLineToLineNode(0,
                                                                               3);

            Assert.AreEqual(m_LineOne,
                            actual.From,
                            "Line");
            Assert.AreEqual(Constants.LineDirection.Forward,
                            actual.FromDirection,
                            "FromDirection");
            Assert.AreEqual(m_LineTwo,
                            actual.To,
                            "To");
            Assert.AreEqual(Constants.LineDirection.Reverse,
                            actual.ToDirection,
                            "ToDirection");
        }

        [Test]
        public void DisposeCallsDisposeTest()
        {
            var disposer = Substitute.For <IDisposer>();

            var converter = new PathToLineToLineNodeConverter(disposer,
                                                              m_Factory,
                                                              m_NodeIndexHelper);

            converter.Dispose();

            disposer.Received().Dispose();
        }

        [Test]
        public void NodesDefaultTest()
        {
            Assert.AreEqual(0,
                            m_Converter.Nodes.Count());
        }

        [Test]
        public void PathDefaultTest()
        {
            Assert.AreEqual(0,
                            m_Converter.Path.Count());
        }

        [Test]
        public void PathRoundtripTest()
        {
            var path = new[]
                       {
                           0,
                           2
                       };

            m_Converter.Path = path;

            NUnitHelper.AssertSequenceEqual(path,
                                            m_Converter.Path,
                                            "Path");
        }

        [Test]
        public void ReleaseConvertersCallsReleaseTest()
        {
            var path = new[]
                       {
                           0,
                           2
                       };

            m_Converter.Path = path;
            m_Converter.Convert();

            m_Converter.ReleaseConverters();

            m_Factory.Received(1).Release(Arg.Any <ILineToLineNodeConverter>());
        }

        [Test]
        public void ReleaseConvertersClearsNodesTest()
        {
            var path = new[]
                       {
                           0,
                           2
                       };

            m_Converter.Path = path;
            m_Converter.Convert();

            m_Converter.ReleaseConverters();

            Assert.AreEqual(0,
                            m_Converter.Nodes.Count());
        }
    }
}