using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.ViewModels.NUnit;

namespace Selkie.WPF.ViewModels.Mapping.NUnit
{
    [TestFixture]
    internal sealed class MapViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_ConverterNodes = Substitute.For <ILineNodeToDisplayLineNodeConverter>();
            m_ConverterStartNodeModel = Substitute.For <INodeModelToDisplayNodeConverter>();
            m_ConverterEndNodeModel = Substitute.For <INodeModelToDisplayNodeConverter>();
            m_ConverterRacetrack = Substitute.For <IRacetrackPathsToFiguresConverter>();
            m_ConverterDirections = Substitute.For <INodesToDisplayNodesConverter>();
            m_LinesModel = Substitute.For <ILinesModel>();
            m_NodesModel = Substitute.For <INodesModel>();
            m_StartNodeModel = Substitute.For <IStartNodeModel>();
            m_EndNodeModel = Substitute.For <IEndNodeModel>();
            m_ShortestPathModel = Substitute.For <IShortestPathModel>();
            m_ShortestPathDirectionModel = Substitute.For <IShortestPathDirectionModel>();
            m_RacetrackModel = Substitute.For <IRacetrackModel>();

            m_Model = CreateModel(m_Dispatcher);
        }

        private MapViewModel CreateModel([NotNull] IApplicationDispatcher dispatcher)
        {
            return new MapViewModel(m_Logger,
                                    m_Bus,
                                    dispatcher,
                                    m_ConverterNodes,
                                    m_ConverterStartNodeModel,
                                    m_ConverterEndNodeModel,
                                    m_ConverterRacetrack,
                                    m_ConverterDirections,
                                    m_LinesModel,
                                    m_NodesModel,
                                    m_StartNodeModel,
                                    m_EndNodeModel,
                                    m_ShortestPathModel,
                                    m_ShortestPathDirectionModel,
                                    m_RacetrackModel);
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private TestImmediateDispatcher m_Dispatcher;
        private ILineNodeToDisplayLineNodeConverter m_ConverterNodes;
        private INodeModelToDisplayNodeConverter m_ConverterStartNodeModel;
        private INodeModelToDisplayNodeConverter m_ConverterEndNodeModel;
        private IRacetrackPathsToFiguresConverter m_ConverterRacetrack;
        private INodesToDisplayNodesConverter m_ConverterDirections;
        private ILinesModel m_LinesModel;
        private INodesModel m_NodesModel;
        private IStartNodeModel m_StartNodeModel;
        private IEndNodeModel m_EndNodeModel;
        private IShortestPathModel m_ShortestPathModel;
        private IShortestPathDirectionModel m_ShortestPathDirectionModel;
        private IRacetrackModel m_RacetrackModel;
        private MapViewModel m_Model;

        [Test]
        public void Constructor_SetsFillBrushToRedForEndNodes_WhenCreated()
        {
            Assert.True(m_ConverterEndNodeModel.FillBrush.Equals(Brushes.Red));
        }

        [Test]
        public void Constructor_SetsStrokeBrushToDarkRedForEndNodes_WhenCreated()
        {
            Assert.True(m_ConverterEndNodeModel.StrokeBrush.Equals(Brushes.DarkRed));
        }

        [Test]
        public void Constructor_SubscribeToColonyFinishedMessageLinesModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <LinesModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToEndNodeModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <EndNodeModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToNodesModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <NodesModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToRacetrackModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <RacetrackModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToShortestPathDirectionModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ShortestPathDirectionModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToShortestPathModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ShortestPathModelChangedMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToStartNodeModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <StartNodeModelChangedMessage, Task>>());
        }

        [Test]
        public void EndNodeModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new EndNodeModelChangedMessage();

            // Act
            model.EndNodeModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateEndNode);
        }

        [Test]
        public void EndNodeModelChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            var message = new EndNodeModelChangedMessage();

            // Act
            m_Model.EndNodeModelChangedHandler(message);

            // Assert
            m_ConverterEndNodeModel.Received().Convert();
        }

        [Test]
        public void EndNodeModelChangedHandler_SetsNodeModel_WhenCalled()
        {
            // Arrange
            var nodeModel = Substitute.For <INodeModel>();
            m_EndNodeModel.Node.Returns(nodeModel);
            var message = new EndNodeModelChangedMessage();

            // Act
            m_Model.EndNodeModelChangedHandler(message);

            // Assert
            Assert.AreEqual(nodeModel,
                            m_ConverterEndNodeModel.NodeModel);
        }

        [Test]
        public void LinesModelChangedMessageHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new LinesModelChangedMessage();

            // Act
            model.LinesModelChangedMessageHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateLines);
        }

        [Test]
        public void NodesModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new NodesModelChangedMessage();

            // Act
            model.NodesModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateNodes);
        }

        [Test]
        public void NodesModelChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            var message = new NodesModelChangedMessage();

            // Act
            m_Model.NodesModelChangedHandler(message);

            // Assert
            m_ConverterNodes.Received().Convert();
        }

        [Test]
        public void NodesModelChangedHandler_SetsNodeModel_WhenCalled()
        {
            // Arrange
            IEnumerable <INodeModel> nodeModels = new[]
                                                  {
                                                      Substitute.For <INodeModel>()
                                                  };

            m_NodesModel.Nodes.Returns(nodeModels);
            var message = new NodesModelChangedMessage();

            // Act
            m_Model.NodesModelChangedHandler(message);

            // Assert
            Assert.AreEqual(nodeModels,
                            m_ConverterNodes.NodeModels);
        }

        [Test]
        public void RacetrackModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new RacetrackModelChangedMessage();

            // Act
            model.RacetrackModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateRacetrack);
        }

        [Test]
        public void ShortestPathDirectionModelChangedHandlerhen_CallsConvert_WhenCalled()
        {
            // Arrange
            var message = new ShortestPathDirectionModelChangedMessage();

            // Act
            m_Model.ShortestPathDirectionModelChangedHandler(message);

            // Assert
            m_ConverterDirections.Received().Convert();
        }

        [Test]
        public void ShortestPathDirectionModelChangedHandlerhen_CallsUpdatePathDirections_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);

            var message = new ShortestPathDirectionModelChangedMessage();

            // Act
            model.ShortestPathDirectionModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdatePathDirections);
        }

        [Test]
        public void ShortestPathDirectionModelChangedHandlerhen_SetsNodeModels_WhenCalled()
        {
            // Arrange
            var message = new ShortestPathDirectionModelChangedMessage();

            // Act
            m_Model.ShortestPathDirectionModelChangedHandler(message);

            // Assert
            Assert.AreEqual(m_ConverterDirections.NodeModels,
                            m_ShortestPathDirectionModel.Nodes);
        }

        [Test]
        public void ShortestPathModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new ShortestPathModelChangedMessage();

            // Act
            model.ShortestPathModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateShortestPath);
        }

        [Test]
        public void StartNodeModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            MapViewModel model = CreateModel(dispatcher);
            var message = new StartNodeModelChangedMessage();

            // Act
            model.StartNodeModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(model.UpdateStartNode);
        }

        [Test]
        public void StartNodeModelChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            var message = new StartNodeModelChangedMessage();

            // Act
            m_Model.StartNodeModelChangedHandler(message);

            // Assert
            m_ConverterStartNodeModel.Received().Convert();
        }

        [Test]
        public void StartNodeModelChangedHandler_SetsNodeModel_WhenCalled()
        {
            // Arrange
            var nodeModel = Substitute.For <INodeModel>();
            m_StartNodeModel.Node.Returns(nodeModel);
            var message = new StartNodeModelChangedMessage();

            // Act
            m_Model.StartNodeModelChangedHandler(message);

            // Assert
            Assert.AreEqual(nodeModel,
                            m_ConverterStartNodeModel.NodeModel);
        }

        [Test]
        public void UpdateEndNode_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "EndNode");

            // Act
            m_Model.UpdateEndNode();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateEndNode_SetsEndNode_WhenCalled()
        {
            // Arrange
            var nodeModel = Substitute.For <IDisplayNode>();
            m_ConverterEndNodeModel.DisplayNode.Returns(nodeModel);

            // Act
            m_Model.UpdateEndNode();

            // Assert
            Assert.AreEqual(nodeModel,
                            m_Model.EndNode);
        }

        [Test]
        public void UpdateLines_AddsDisplayLines_WhenCalled()
        {
            // Arrange
            IEnumerable <IDisplayLine> lines = new[]
                                               {
                                                   Substitute.For <IDisplayLine>()
                                               };

            m_LinesModel.Lines.Returns(lines);

            // Act
            m_Model.UpdateLines();

            // Assert
            Assert.True(lines.SequenceEqual(m_Model.Lines));
        }

        [Test]
        public void UpdateLines_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "Lines");

            // Act
            m_Model.UpdateLines();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateNodes_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "Nodes");

            // Act
            m_Model.UpdateNodes();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateNodes_SetsNodes_WhenCalled()
        {
            // Arrange
            IDisplayNode[] nodeModel =
            {
                Substitute.For <IDisplayNode>()
            };
            m_ConverterNodes.DisplayNodes.Returns(nodeModel);

            // Act
            m_Model.UpdateNodes();

            // Assert
            Assert.AreEqual(nodeModel,
                            m_Model.Nodes);
        }

        [Test]
        public void UpdatePathDirections_AddsDisplayNodes_WhenCalled()
        {
            // Arrange
            var displayNodes = new[]
                               {
                                   Substitute.For <IDisplayNode>()
                               };
            m_ConverterDirections.DisplayNodes.Returns(displayNodes);

            // Act
            m_Model.UpdatePathDirections();

            // Assert
            Assert.True(displayNodes.SequenceEqual(m_Model.PathDirections));
        }

        [Test]
        public void UpdatePathDirections_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "PathDirections");

            // Act
            m_Model.UpdatePathDirections();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateRacetrack_CallsConvert_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.UpdateRacetrack();

            // Assert
            m_ConverterRacetrack.Received().Convert();
        }

        [Test]
        public void UpdateRacetrack_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "Racetracks");

            // Act
            m_Model.UpdateRacetrack();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateRacetrack_SetsPath_WhenCalled()
        {
            // Arrange
            IEnumerable <IPath> paths = new[]
                                        {
                                            Substitute.For <IPath>()
                                        };

            m_RacetrackModel.Paths.Returns(paths);

            // Act
            m_Model.UpdateRacetrack();

            // Assert
            Assert.AreEqual(m_ConverterRacetrack.Paths,
                            m_RacetrackModel.Paths);
        }

        [Test]
        public void UpdateRacetrack_SetsPathRacetracks_WhenCalled()
        {
            // Arrange
            IEnumerable <PathFigureCollection> paths = new[]
                                                       {
                                                           new PathFigureCollection()
                                                       };

            m_ConverterRacetrack.Figures.Returns(paths);

            // Act
            m_Model.UpdateRacetrack();

            // Assert
            Assert.AreEqual(m_Model.Racetracks,
                            paths);
        }

        [Test]
        public void UpdateShortestPath_AddsPath_WhenCalled()
        {
            // Arrange
            IEnumerable <IDisplayLine> lines = new[]
                                               {
                                                   Substitute.For <IDisplayLine>()
                                               };

            m_ShortestPathModel.Path.Returns(lines);

            // Act
            m_Model.UpdateShortestPath();

            // Assert
            Assert.True(lines.SequenceEqual(m_Model.ShortestPath));
        }

        [Test]
        public void UpdateShortestPath_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "ShortestPath");

            // Act
            m_Model.UpdateShortestPath();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateStartNode_NotifyPropertyChanged_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "StartNode");

            // Act
            m_Model.UpdateStartNode();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateStartNode_SetsEndNode_WhenCalled()
        {
            // Arrange
            var nodeModel = Substitute.For <IDisplayNode>();
            m_ConverterStartNodeModel.DisplayNode.Returns(nodeModel);

            // Act
            m_Model.UpdateStartNode();

            // Assert
            Assert.AreEqual(nodeModel,
                            m_Model.StartNode);
        }
    }
}