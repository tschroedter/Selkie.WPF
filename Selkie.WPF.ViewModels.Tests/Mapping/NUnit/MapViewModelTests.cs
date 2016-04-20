using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.ViewModels.Mapping;
using Selkie.WPF.ViewModels.Mapping.Handlers;
using Selkie.WPF.ViewModels.Tests.NUnit;

namespace Selkie.WPF.ViewModels.Tests.Mapping.NUnit
{
    [TestFixture]
    internal sealed class MapViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Handler = Substitute.For <IMapViewModelMessageHandler>();

            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_Handlers = new[]
                         {
                             m_Handler
                         };

            m_Sut = CreateModel(m_Dispatcher);
        }

        private MapViewModel CreateModel([NotNull] IApplicationDispatcher dispatcher)
        {
            return new MapViewModel(m_Bus,
                                    dispatcher,
                                    m_Handlers);
        }

        private ISelkieInMemoryBus m_Bus;
        private TestImmediateDispatcher m_Dispatcher;
        private MapViewModel m_Sut;
        private IMapViewModelMessageHandler[] m_Handlers;
        private IMapViewModelMessageHandler m_Handler;

        [Test]
        public void Constructor_SendsLinesModelLinesRequestMessage_WhenCreated()
        {
            m_Bus.Received().PublishAsync(Arg.Any <LinesModelLinesRequestMessage>());
        }

        [Test]
        public void Constructor_SetsModelInHandler_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Handler.Received().SetMapViewModel(m_Sut);
        }

        [Test]
        public void LinesModelChangedMessageHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetLines(new IDisplayLine[0]);

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void SetDirections_AddsDisplayNodes_WhenCalled()
        {
            // Arrange
            var displayNodes = new[]
                               {
                                   Substitute.For <IDisplayNode>()
                               };

            // Act
            m_Sut.SetDirections(displayNodes);

            // Assert
            Assert.True(displayNodes.SequenceEqual(m_Sut.PathDirections));
        }

        [Test]
        public void SetDirections_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "PathDirections");

            // Act
            m_Sut.SetDirections(new IDisplayNode[0]);

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void SetEndNode_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetEndNode(Substitute.For <IDisplayNode>());

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void SetEndNodeUpdateEndNode_SetsEndNode_WhenCalled()
        {
            // Arrange
            var expected = Substitute.For <IDisplayNode>();

            // Act
            m_Sut.SetEndNode(expected);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.EndNode);
        }

        [Test]
        public void SetLines_AddsDisplayLines_WhenCalled()
        {
            // Arrange
            IEnumerable <IDisplayLine> lines = new[]
                                               {
                                                   Substitute.For <IDisplayLine>()
                                               };

            // Act
            m_Sut.SetLines(lines);

            // Assert
            Assert.True(lines.SequenceEqual(m_Sut.Lines));
        }

        [Test]
        public void SetLines_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "Lines");

            // Act
            m_Sut.SetLines(new List <IDisplayLine>());

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void SetNodes_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetNodes(new List <IDisplayNode>());

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void SetNodes_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "Nodes");

            // Act
            m_Sut.SetNodes(new IDisplayNode[0]);

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void SetNodes_SetsNodes_WhenCalled()
        {
            // Arrange
            IDisplayNode[] displayNodes =
            {
                Substitute.For <IDisplayNode>()
            };

            // Act
            m_Sut.SetNodes(displayNodes);

            // Assert
            Assert.AreEqual(displayNodes,
                            m_Sut.Nodes);
        }

        [Test]
        public void SetRacetrack_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetRacetracks(new PathFigureCollection[0]);

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void SetRacetrack_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "Racetracks");

            // Act
            m_Sut.SetRacetracks(new PathFigureCollection[0]);

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void SetStartNode_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetStartNode(Substitute.For <IDisplayNode>());

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void SetStartNode_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "StartNode");

            // Act
            m_Sut.SetStartNode(Substitute.For <IDisplayNode>());

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void SetStartNode_SetsNodeModel_WhenCalled()
        {
            // Arrange
            var expected = Substitute.For <IDisplayNode>();

            // Act
            m_Sut.SetStartNode(expected);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.StartNode);
        }

        [Test]
        public void SetStartNode_SetsStartNode_WhenCalled()
        {
            // Arrange
            var expected = Substitute.For <IDisplayNode>();

            // Act
            m_Sut.SetStartNode(expected);

            // Assert
            Assert.AreEqual(expected,
                            m_Sut.StartNode);
        }

        [Test]
        public void ShortestPathModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            // Act
            model.SetshortestPath(new List <IDisplayLine>());

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void UpdateEndNode_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "EndNode");

            // Act
            m_Sut.SetEndNode(Substitute.For <IDisplayNode>());

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateRacetrack_SetsPathRacetracks_WhenCalled()
        {
            // Arrange
            IEnumerable <PathFigureCollection> expected = new[]
                                                          {
                                                              new PathFigureCollection()
                                                          };

            // Act
            m_Sut.SetRacetracks(expected);

            // Assert
            Assert.AreEqual(m_Sut.Racetracks,
                            expected);
        }

        [Test]
        public void UpdateShortestPath_AddsPath_WhenCalled()
        {
            // Arrange
            List <IDisplayLine> expected = new[]
                                           {
                                               Substitute.For <IDisplayLine>()
                                           }.ToList();

            // Act
            m_Sut.SetshortestPath(expected);

            // Assert
            Assert.True(expected.SequenceEqual(m_Sut.ShortestPath));
        }

        [Test]
        public void UpdateShortestPath_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Sut,
                                                     "ShortestPath");

            // Act
            m_Sut.SetshortestPath(new List <IDisplayLine>());

            // Assert
            Assert.True(test.IsExpectedNotified);
        }
    }
}