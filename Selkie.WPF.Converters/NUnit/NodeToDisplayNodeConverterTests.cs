using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.Common;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodeToDisplayNodeConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Disposer = Substitute.For <IDisposer>();
            m_Factory = Substitute.For <IDisplayNodeFactory>();

            m_Converter = new NodeModelToDisplayNodeConverter(m_Disposer,
                                                              m_Factory);
        }

        [TearDown]
        public void Teardown()
        {
            m_Converter.Dispose();
        }

        private IDisplayNodeFactory m_Factory;
        private NodeModelToDisplayNodeConverter m_Converter;
        private IDisposer m_Disposer;

        private static INodeModel CreateNodeModel()
        {
            var nodeModel = Substitute.For <INodeModel>();
            nodeModel.Id.Returns(1);
            nodeModel.X.Returns(1.0);
            nodeModel.Y.Returns(2.0);
            nodeModel.DirectionAngle.Returns(Angle.For45Degrees);
            return nodeModel;
        }

        [Test]
        public void ConvertCallReleaseTest()
        {
            m_Converter.NodeModel = CreateNodeModel();

            m_Converter.Convert();
            m_Converter.Convert();

            m_Factory.ReceivedWithAnyArgs().Release(null);
        }

        [Test]
        public void ConvertCallsCreateTest()
        {
            INodeModel nodeModel = CreateNodeModel();

            m_Converter.NodeModel = nodeModel;
            m_Converter.Convert();

            m_Factory.Received().Create(1,
                                        1.0,
                                        2.0,
                                        45.0,
                                        NodeModelToDisplayNodeConverter.DefaultRadius,
                                        NodeModelToDisplayNodeConverter.DefaultFill,
                                        NodeModelToDisplayNodeConverter.DefaultStroke,
                                        NodeModelToDisplayNodeConverter.DefaultStrokeThickness);
        }

        [Test]
        public void DefaultDisplayNodeTest()
        {
            Assert.NotNull(m_Converter.DisplayNode);
        }

        [Test]
        public void DefaultFillBrushRoundtripTest()
        {
            SolidColorBrush brush = Brushes.Beige;

            m_Converter.FillBrush = brush;

            Assert.AreEqual(brush,
                            m_Converter.FillBrush);
        }

        [Test]
        public void DefaultFillBrushTest()
        {
            Assert.AreEqual(NodeModelToDisplayNodeConverter.DefaultFill,
                            m_Converter.FillBrush);
        }

        [Test]
        public void DefaultStrokeBrushRoundtripTest()
        {
            SolidColorBrush brush = Brushes.Beige;

            m_Converter.StrokeBrush = brush;

            Assert.AreEqual(brush,
                            m_Converter.StrokeBrush);
        }

        [Test]
        public void DefaultStrokeBrushTest()
        {
            Assert.AreEqual(NodeModelToDisplayNodeConverter.DefaultStroke,
                            m_Converter.StrokeBrush);
        }

        [Test]
        public void DisposCallsDisposeTest()
        {
            m_Converter.NodeModel = CreateNodeModel();
            m_Converter.Convert();

            m_Converter.ReleaseDisplayNode();

            m_Factory.ReceivedWithAnyArgs().Release(null);
        }

        [Test]
        public void DisposeDoesNotCallDisposeForUnkownNodeTest()
        {
            m_Converter.ReleaseDisplayNode();

            m_Factory.DidNotReceiveWithAnyArgs().Release(null);
        }

        [Test]
        public void DisposeTest()
        {
            var converter = new NodeModelToDisplayNodeConverter(m_Disposer,
                                                                m_Factory);

            converter.Dispose();

            m_Disposer.Received().Dispose();
        }

        [Test]
        public void NodeModelRoundtripTest()
        {
            var nodeModel = Substitute.For <INodeModel>();

            m_Converter.NodeModel = Substitute.For <INodeModel>();
            m_Converter.NodeModel = nodeModel;

            Assert.AreEqual(nodeModel,
                            m_Converter.NodeModel);
        }

        [Test]
        public void NodeModelTest()
        {
            var nodeModel = Substitute.For <INodeModel>();

            m_Converter.NodeModel = nodeModel;

            Assert.AreEqual(nodeModel,
                            m_Converter.NodeModel);
        }
    }
}