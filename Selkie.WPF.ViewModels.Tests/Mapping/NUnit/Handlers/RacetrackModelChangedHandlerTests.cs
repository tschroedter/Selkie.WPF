using System.Collections.Generic;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Interfaces;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels.Tests.Mapping.NUnit.Handlers
{
    [TestFixture]
    internal sealed class RacetrackModelChangedHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <IRacetrackPathsToFiguresConverter>();
            m_Model = Substitute.For <IRacetrackModel>();
            m_MapViewModel = Substitute.For <IMapViewModel>();

            m_Sut = new RacetrackModelChangedHandler(m_Logger,
                                                     m_Bus,
                                                     m_Converter,
                                                     m_Model);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private IRacetrackModel m_Model;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private IRacetrackPathsToFiguresConverter m_Converter;
        private RacetrackModelChangedHandler m_Sut;
        private IMapViewModel m_MapViewModel;

        [Test]
        public void Handle_CallsConverter_WhenCalled()
        {
            // Arrange
            var message = new RacetrackModelChangedMessage();

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
                               new PathFigureCollection()
                           };

            var message = new RacetrackModelChangedMessage();

            m_Converter.Figures.Returns(expected);

            // Act
            m_Sut.Handle(message);

            // Assert
            m_MapViewModel.Received()
                          .SetRacetracks(expected);
        }

        [Test]
        public void Handle_SetsNodeModelsInConverter_WhenCalled()
        {
            // Arrange
            IEnumerable <IPath> expected = new[]
                                           {
                                               Substitute.For <IPath>()
                                           };

            m_Model.Paths.Returns(expected);

            var message = new RacetrackModelChangedMessage();

            // Act
            m_Sut.Handle(message);

            // Assert
            Assert.AreEqual(expected,
                            m_Converter.Paths);
        }
    }
}