using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodesToDisplayNodesConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Factory = Substitute.For <IDisplayNodeFactory>();

            m_Converter = new NodesToDisplayNodesConverter(m_Factory);
        }

        [TearDown]
        public void Teardown()
        {
            m_Converter.Dispose();
        }

        private IDisplayNodeFactory m_Factory;
        private NodesToDisplayNodesConverter m_Converter;

        private IEnumerable <INodeModel> CreateNodeModels()
        {
            var nodeModels = new[]
                             {
                                 CreateNodeModel(),
                                 CreateNodeModel()
                             };

            return nodeModels;
        }

        private static INodeModel CreateNodeModel()
        {
            var nodeModel = Substitute.For <INodeModel>();
            nodeModel.Id.Returns(2);
            nodeModel.X.Returns(3.0);
            nodeModel.Y.Returns(4.0);
            nodeModel.DirectionAngle.Returns(Angle.For45Degrees);

            return nodeModel;
        }

        [Test]
        public void AddDisplayNodesAddsTest()
        {
            m_Converter.NodeModels = CreateNodeModels();

            m_Converter.AddDisplayNodes();

            IEnumerable <IDisplayNode> actual = m_Converter.DisplayNodes;

            Assert.AreEqual(2,
                            actual.Count());
        }

        [Test]
        public void ConvertCallsAddDisplayNodesTest()
        {
            m_Converter.NodeModels = CreateNodeModels();

            m_Converter.Convert();

            IEnumerable <IDisplayNode> actual = m_Converter.DisplayNodes;

            Assert.AreEqual(2,
                            actual.Count());
        }

        [Test]
        public void ConvertCallsReleaseDisplayNodesTest()
        {
            m_Converter.NodeModels = CreateNodeModels();
            m_Converter.Convert();

            m_Converter.NodeModels = new INodeModel[]
                                     {
                                     };
            m_Converter.Convert();

            m_Factory.Received(2).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void CreateDisplayNodeCallsCreateTest()
        {
            var nodeModel = Substitute.For <INodeModel>();
            nodeModel.Id.Returns(1);
            nodeModel.X.Returns(1.0);
            nodeModel.Y.Returns(2.0);
            nodeModel.DirectionAngle.Returns(Angle.For45Degrees);

            m_Converter.CreateDisplayNode(nodeModel);

            m_Factory.Received().Create(1,
                                        1.0,
                                        2.0,
                                        -Angle.For45Degrees.Degrees,
                                        NodesToDisplayNodesConverter.DefaultRadius,
                                        NodesToDisplayNodesConverter.DefaultFill,
                                        NodesToDisplayNodesConverter.DefaultStroke,
                                        NodesToDisplayNodesConverter.DefaultStrokeThickness);
        }

        [Test]
        public void CreateDisplayNodeReturnsDisplayNodeTest()
        {
            INodeModel nodeModel = CreateNodeModel();

            IDisplayNode actual = m_Converter.CreateDisplayNode(nodeModel);

            Assert.NotNull(actual);
        }

        [Test]
        public void DefaultDisplayNodesTest()
        {
            Assert.NotNull(m_Converter.DisplayNodes);
        }

        [Test]
        public void DefaultNodesTest()
        {
            Assert.NotNull(m_Converter.NodeModels);
        }

        [Test]
        public void DisposeCallsReleaseDisplayNodesTest()
        {
            var factory = Substitute.For <IDisplayNodeFactory>();
            var converter = new NodesToDisplayNodesConverter(factory)
                            {
                                NodeModels = CreateNodeModels()
                            };

            converter.Convert();
            converter.Dispose();

            factory.Received(2).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void NodesRoundtripTest()
        {
            IEnumerable <INodeModel> nodes = CreateNodeModels();

            m_Converter.NodeModels = new[]
                                     {
                                         Substitute.For <INodeModel>()
                                     };

            m_Converter.NodeModels = nodes;

            Assert.AreEqual(nodes,
                            m_Converter.NodeModels);
        }

        [Test]
        public void NodesTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <INodeModel>()
                        };

            m_Converter.NodeModels = nodes;

            Assert.AreEqual(nodes,
                            m_Converter.NodeModels);
        }

        [Test]
        public void ReleaseDisplayNodesCallsReleaseTest()
        {
            m_Converter.NodeModels = CreateNodeModels();

            m_Converter.Convert();
            m_Converter.ReleaseDisplayNodes();

            m_Factory.Received(2).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void ReleaseDisplayNodesClearsDisplayNodesTest()
        {
            m_Converter.NodeModels = CreateNodeModels();

            m_Converter.Convert();
            m_Converter.ReleaseDisplayNodes();

            IEnumerable <IDisplayNode> actual = m_Converter.DisplayNodes;

            Assert.AreEqual(0,
                            actual.Count());
        }
    }
}