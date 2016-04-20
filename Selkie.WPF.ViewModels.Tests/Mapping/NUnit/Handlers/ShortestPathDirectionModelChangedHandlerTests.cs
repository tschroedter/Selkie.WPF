using System.Collections.Generic;
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
    internal sealed class ShortestPathDirectionModelChangedHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <INodesToDisplayNodesConverter>();
            m_Model = Substitute.For <IShortestPathDirectionModel>();
            m_MapViewModel = Substitute.For <IMapViewModel>();

            m_Sut = new ShortestPathDirectionModelChangedHandler(m_Logger,
                                                                 m_Bus,
                                                                 m_Converter,
                                                                 m_Model);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private IShortestPathDirectionModel m_Model;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private INodesToDisplayNodesConverter m_Converter;
        private ShortestPathDirectionModelChangedHandler m_Sut;
        private IMapViewModel m_MapViewModel;

        [Test]
        public void Handle_CallsConverter_WhenCalled()
        {
            // Arrange
            var message = new ShortestPathDirectionModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void Handle_SetsPathInConverter_WhenCalled()
        {
            // Arrange
            IEnumerable <INodeModel> expected = new[]
                                                {
                                                    Substitute.For <INodeModel>()
                                                };

            m_Model.Nodes.Returns(expected);

            var message = new ShortestPathDirectionModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Converter.NodeModels);
        }

        [Test]
        public void Handle_SetsRacetracksInModel_WhenCalled()
        {
            // Arrange
            IEnumerable <IDisplayNode> expected = new[]
                                                  {
                                                      Substitute.For <IDisplayNode>()
                                                  };

            var message = new ShortestPathDirectionModelChangedMessage();

            m_Converter.DisplayNodes.Returns(expected);

            // Act
            m_Sut.Handle(message);

            // Assert
            m_MapViewModel.Received()
                          .SetDirections(expected);
        }
    }
}