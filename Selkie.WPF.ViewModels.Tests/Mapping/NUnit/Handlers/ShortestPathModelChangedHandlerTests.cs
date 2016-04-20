using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels.Tests.Mapping.NUnit.Handlers
{
    [TestFixture]
    internal sealed class ShortestPathModelChangedHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_MapViewModel = Substitute.For <IMapViewModel>();
            m_ShortestPathModel = Substitute.For <IShortestPathModel>();

            m_Sut = new ShortestPathModelChangedHandler(m_Logger,
                                                        m_Bus,
                                                        m_ShortestPathModel);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private IShortestPathModel m_ShortestPathModel;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private IMapViewModel m_MapViewModel;
        private ShortestPathModelChangedHandler m_Sut;

        [Test]
        public void Handle_SetsShortestPathInModel_WhenCalled()
        {
            // Arrange
            List <IDisplayLine> list = new[]
                                       {
                                           Substitute.For <IDisplayLine>()
                                       }.ToList();

            var message = new ShortestPathModelChangedMessage();

            m_ShortestPathModel.Path.Returns(list);

            // Act
            m_Sut.Handle(message);

            // Assert
            m_MapViewModel.Received()
                          .SetshortestPath(Arg.Is <List <IDisplayLine>>(x => list.Count == x.Count));
        }
    }
}