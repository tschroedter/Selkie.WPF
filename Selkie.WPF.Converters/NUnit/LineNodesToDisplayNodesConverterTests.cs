using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LineNodesToDisplayNodesConverterTests
    {
        [TearDown]
        public void Teardown()
        {
            m_Converter.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            m_Factory = CreateFactory();

            m_Converter = new LineNodesToDisplayNodesConverter(m_Factory);
        }

        private IDisplayNodeFactory CreateFactory()
        {
            var factory = Substitute.For <IDisplayNodeFactory>();
            factory.Create(Arg.Any <int>(),
                           Arg.Any <double>(),
                           Arg.Any <double>(),
                           Arg.Any <double>(),
                           Arg.Any <double>(),
                           Arg.Any <SolidColorBrush>(),
                           Arg.Any <SolidColorBrush>(),
                           Arg.Any <double>()).Returns(x =>
                                                       {
                                                           var displayNode = Substitute.For <IDisplayNode>();

                                                           displayNode.Id.Returns(( int ) x [ 0 ]);
                                                           displayNode.X.Returns(( double ) x [ 1 ]);
                                                           displayNode.Y.Returns(( double ) x [ 2 ]);
                                                           displayNode.DirectionAngle.Returns(( double ) x [ 3 ]);
                                                           displayNode.Radius.Returns(( double ) x [ 4 ]);
                                                           displayNode.Stroke.Returns(( SolidColorBrush ) x [ 5 ]);
                                                           displayNode.Fill.Returns(( SolidColorBrush ) x [ 6 ]);
                                                           displayNode.StrokeThickness.Returns(( double ) x [ 7 ]);

                                                           return displayNode;
                                                       });

            return factory;
        }

        private LineNodesToDisplayNodesConverter m_Converter;
        private IDisplayNodeFactory m_Factory;

        private IEnumerable <INodeModel> CreateNodeModels()
        {
            var modelOne = Substitute.For <INodeModel>();
            modelOne.Id.Returns(2);
            modelOne.X.Returns(3.0);
            modelOne.Y.Returns(4.0);
            modelOne.DirectionAngle.Returns(Angle.For225Degrees);

            var modelTwo = Substitute.For <INodeModel>();
            modelTwo.Id.Returns(2);
            modelTwo.X.Returns(3.0);
            modelTwo.Y.Returns(4.0);
            modelTwo.DirectionAngle.Returns(Angle.For225Degrees);

            var models = new[]
                         {
                             modelOne,
                             modelTwo
                         };

            return models;
        }

        [Test]
        public void ConvertCallsLoadDisplayNodesTest()
        {
            m_Converter.NodeModels = CreateNodeModels();
            m_Converter.Convert();

            int count = m_Converter.NodeModels.Count();

            IEnumerable <IDisplayNode> actual = m_Converter.DisplayNodes;

            Assert.AreEqual(count,
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

            int count = m_Converter.NodeModels.Count();

            m_Factory.Received(count).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void DefaultDisplayNodesTest()
        {
            Assert.NotNull(m_Converter.DisplayNodes);
        }

        [Test]
        public void DisposeCallsReleaseDisplayNodesTest()
        {
            var converter = new LineNodesToDisplayNodesConverter(m_Factory)
                            {
                                NodeModels = CreateNodeModels()
                            };

            int count = converter.NodeModels.Count();

            converter.Convert();

            converter.Dispose();

            m_Factory.Received(count).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void LoadDisplayNodesCallsCreateTest()
        {
            INodeModel[] nodeModels = CreateNodeModels().ToArray();

            m_Converter.NodeModels = nodeModels;

            m_Converter.LoadDisplayNodes();

            int count = nodeModels.Count();

            m_Factory.Received().Received(count).Create(Arg.Any <int>(),
                                                        Arg.Any <double>(),
                                                        Arg.Any <double>(),
                                                        Arg.Any <double>(),
                                                        Arg.Any <double>(),
                                                        Arg.Any <SolidColorBrush>(),
                                                        Arg.Any <SolidColorBrush>(),
                                                        Arg.Any <double>());
        }

        [Test]
        public void LoadDisplayNodesCountTest()
        {
            INodeModel[] nodeModels = CreateNodeModels().ToArray();

            m_Converter.NodeModels = nodeModels;

            m_Converter.LoadDisplayNodes();

            IEnumerable <IDisplayNode> actual = m_Converter.DisplayNodes;

            Assert.AreEqual(nodeModels.Count(),
                            actual.Count());
        }

        [Test]
        public void LoadDisplayNodesDisplayNodeLastTest()
        {
            INodeModel[] nodeModels = CreateNodeModels().ToArray();

            m_Converter.NodeModels = nodeModels;

            m_Converter.LoadDisplayNodes();

            INodeModel expected = nodeModels.Last();
            IDisplayNode actual = m_Converter.DisplayNodes.Last();

            Assert.AreEqual(expected.Id,
                            actual.Id,
                            "Id");
            Assert.AreEqual(expected.X,
                            actual.X,
                            "X");
            Assert.AreEqual(expected.Y,
                            actual.Y,
                            "Y");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultStroke,
                            actual.Stroke,
                            "Stroke");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultFill,
                            actual.Fill,
                            "`Fill");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultRadius,
                            actual.Radius,
                            "Radius");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultStrokeThickness,
                            actual.StrokeThickness,
                            "Radius");
        }

        [Test]
        public void LoadDisplayNodesDisplayNodeOneTest()
        {
            INodeModel[] nodeModels = CreateNodeModels().ToArray();

            m_Converter.NodeModels = nodeModels;

            m_Converter.LoadDisplayNodes();

            INodeModel expected = nodeModels.First();
            IDisplayNode actual = m_Converter.DisplayNodes.First();

            Assert.AreEqual(expected.Id,
                            actual.Id,
                            "Id");
            Assert.AreEqual(expected.X,
                            actual.X,
                            "X");
            Assert.AreEqual(expected.Y,
                            actual.Y,
                            "Y");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultStroke,
                            actual.Stroke,
                            "Stroke");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultFill,
                            actual.Fill,
                            "`Fill");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultRadius,
                            actual.Radius,
                            "Radius");
            Assert.AreEqual(LineNodesToDisplayNodesConverter.DefaultStrokeThickness,
                            actual.StrokeThickness,
                            "Radius");
        }

        [Test]
        public void NodeModelsRoundtripTest()
        {
            m_Converter.NodeModels = CreateNodeModels();

            var nodeModels = new[]
                             {
                                 Substitute.For <INodeModel>()
                             };

            m_Converter.NodeModels = nodeModels;

            Assert.AreEqual(nodeModels,
                            m_Converter.NodeModels);
        }

        [Test]
        public void NodeModelsSetTest()
        {
            IEnumerable <INodeModel> nodeModels = CreateNodeModels();

            m_Converter.NodeModels = nodeModels;

            Assert.AreEqual(nodeModels,
                            m_Converter.NodeModels);
        }

        [Test]
        public void ReleaseDisplayNodesCallsReleaseTest()
        {
            m_Converter.NodeModels = CreateNodeModels();
            m_Converter.Convert();

            m_Converter.ReleaseDisplayNodes();

            int count = m_Converter.NodeModels.Count();

            m_Factory.Received(count).Release(Arg.Any <IDisplayNode>());
        }

        [Test]
        public void ReleaseDisplayNodesClearsListTest()
        {
            m_Converter.NodeModels = CreateNodeModels();
            m_Converter.Convert();

            m_Converter.ReleaseDisplayNodes();

            int actual = m_Converter.DisplayNodes.Count();

            Assert.AreEqual(0,
                            actual);
        }
    }
}