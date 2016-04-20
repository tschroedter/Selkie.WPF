using System.Windows.Media;
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
    internal sealed class EndNodeModelChangeHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <INodeModelToDisplayNodeConverter>();
            m_Model = Substitute.For <IEndNodeModel>();
            m_MapViewModel = Substitute.For <IMapViewModel>();

            m_Sut = new EndNodeModelChangeHandler(m_Logger,
                                                  m_Bus,
                                                  m_Converter,
                                                  m_Model);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private IEndNodeModel m_Model;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private INodeModelToDisplayNodeConverter m_Converter;
        private EndNodeModelChangeHandler m_Sut;
        private IMapViewModel m_MapViewModel;

        [Test]
        public void Constructor_SetsFillBrushToRedForEndNodes_WhenCreated()
        {
            Assert.True(m_Converter.FillBrush.Equals(Brushes.Red));
        }

        [Test]
        public void Constructor_SetsStrokeBrushToDarkRedForEndNodes_WhenCreated()
        {
            Assert.True(m_Converter.StrokeBrush.Equals(Brushes.DarkRed));
        }

        [Test]
        public void Handle_CallsConverter_WhenCalled()
        {
            // Arrange
            var message = new EndNodeModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void Handle_SetsDirectionsInModel_WhenCalled()
        {
            // Arrange
            var expected = Substitute.For <IDisplayNode>();

            var message = new EndNodeModelChangedMessage();

            m_Converter.DisplayNode.Returns(expected);

            // Act
            m_Sut.Handle(message);

            // Assert
            m_MapViewModel.Received()
                          .SetEndNode(expected);
        }

        [Test]
        public void Handle_SetsNodeModelsInConverter_WhenCalled()
        {
            // Arrange
            var expected = Substitute.For <INodeModel>();

            m_Model.Node.Returns(expected);

            var message = new EndNodeModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Converter.NodeModel);
        }
    }
}