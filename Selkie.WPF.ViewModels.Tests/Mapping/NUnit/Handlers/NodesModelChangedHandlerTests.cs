using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels.Tests.Mapping.NUnit.Handlers
{
    [TestFixture]
    internal sealed class NodesModelChangedHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <ILineNodeToDisplayLineNodeConverter>();
            m_Model = Substitute.For <INodesModel>();
            m_MapViewModel = Substitute.For <IMapViewModel>();

            m_Sut = new NodesModelChangedHandler(m_Logger,
                                                 m_Bus,
                                                 m_Converter,
                                                 m_Model);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private INodesModel m_Model;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private ILineNodeToDisplayLineNodeConverter m_Converter;
        private NodesModelChangedHandler m_Sut;
        private IMapViewModel m_MapViewModel;

        [Test]
        public void Handle_CallsConverter_WhenCalled()
        {
            // Arrange
            var message = new NodesModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void Handle_SetsDirectionsInModel_WhenCalled()
        {
            // Arrange
            var expected = new[]
                           {
                               Substitute.For <IDisplayNode>()
                           };

            var message = new NodesModelChangedMessage();

            m_Converter.DisplayNodes.Returns(expected);

            // Act
            m_Sut.Handle(message);

            // Assert
            m_MapViewModel.Received()
                          .SetNodes(expected);
        }

        [Test]
        public void Handle_SetsNodeModelsInConverter_WhenCalled()
        {
            // Arrange
            var expected = new[]
                           {
                               Substitute.For <INodeModel>()
                           };

            m_Model.Nodes.Returns(expected);

            var message = new NodesModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Converter.NodeModels);
        }
    }
}