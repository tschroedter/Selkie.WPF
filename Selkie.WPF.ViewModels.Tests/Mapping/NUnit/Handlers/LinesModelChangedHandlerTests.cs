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
    internal sealed class LinesModelChangedHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_MapViewModel = Substitute.For <IMapViewModel>();
            m_LinesModel = Substitute.For <ILinesModel>();

            m_Sut = new LinesModelChangedHandler(m_Logger,
                                                 m_Bus,
                                                 m_LinesModel);

            m_Sut.SetMapViewModel(m_MapViewModel);
        }

        private ILinesModel m_LinesModel;
        private ISelkieInMemoryBus m_Bus;
        private ISelkieLogger m_Logger;
        private IMapViewModel m_MapViewModel;
        private LinesModelChangedHandler m_Sut;

        [Test]
        public void Handle_SetsLinesInModel_WhenCalled()
        {
            // Arrange
            List <IDisplayLine> lines = new[]
                                        {
                                            Substitute.For <IDisplayLine>()
                                        }.ToList();

            m_LinesModel.Lines.Returns(lines);

            // Act
            m_Sut.Handle(new LinesModelChangedMessage());

            // Assert
            m_MapViewModel.Received()
                          .SetLines(lines);
        }
    }
}