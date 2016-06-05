using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Pheromones;

namespace Selkie.WPF.ViewModels.Tests.Pheromones
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PheromonesViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = Substitute.For <IApplicationDispatcher>();
            m_PheromonesModel = Substitute.For <IPheromonesModel>();
            m_GrayscaleConverter = Substitute.For <IGrayscaleConverter>();
            m_ImageSourceConverter = Substitute.For <IBitmapSourceConverter>();

            m_Model = new PheromonesViewModel(m_Bus,
                                              m_Dispatcher,
                                              m_PheromonesModel,
                                              m_GrayscaleConverter,
                                              m_ImageSourceConverter);
        }

        private PheromonesViewModel m_Model;
        private ISelkieInMemoryBus m_Bus;
        private IApplicationDispatcher m_Dispatcher;
        private IPheromonesModel m_PheromonesModel;
        private IGrayscaleConverter m_GrayscaleConverter;
        private IBitmapSourceConverter m_ImageSourceConverter;

        [Test]
        public void Average_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Model.Average);
        }

        [Test]
        public void Constructor_SubscribeToPheromonesModelChangedMessage_WhenCreated()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <PheromonesModelChangedMessage>>());
        }

        [Test]
        public void GenerateImageSource_CallsConvertOnGrayscaleConverter_WhenCalled()
        {
            // Arrange
            m_GrayscaleConverter.ClearReceivedCalls();

            // Act
            m_Model.GenerateImageSource();

            // Assert
            m_GrayscaleConverter.Received().Convert();
        }

        [Test]
        public void GenerateImageSource_CallsConvertOnImageSourceConverter_WhenCalled()
        {
            // Arrange
            m_ImageSourceConverter.ClearReceivedCalls();

            // Act
            m_Model.GenerateImageSource();

            // Assert
            m_ImageSourceConverter.Received().Convert();
        }

        [Test]
        public void GenerateImageSource_SetsData_WhenCalled()
        {
            // Arrange
            var values = new List <List <int>>
                         {
                             new List <int>()
                         };

            m_GrayscaleConverter.Grayscale.Returns(values);

            // Act
            m_Model.GenerateImageSource();

            // Assert
            Assert.AreEqual(values,
                            m_ImageSourceConverter.Data);
        }

        [Test]
        public void GenerateImageSource_SetsMaximum_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Maximum.Returns(1.0);

            // Act
            m_Model.GenerateImageSource();

            // Assert
            Assert.AreEqual(1.0,
                            m_GrayscaleConverter.Maximum);
        }

        [Test]
        public void GenerateImageSource_SetsValues_WhenCalled()
        {
            // Arrange
            var values = new[]
                         {
                             new[]
                             {
                                 0.0,
                                 0.1
                             },
                             new[]
                             {
                                 0.2,
                                 0.3
                             }
                         };

            m_PheromonesModel.Values.Returns(values);

            // Act
            m_Model.GenerateImageSource();

            // Assert
            Assert.AreEqual(values,
                            m_GrayscaleConverter.Pheromones);
        }

        [Test]
        public void GenerateImageSourceSetsMinimum_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Minimum.Returns(1.0);

            // Act
            m_Model.GenerateImageSource();

            // Assert
            Assert.AreEqual(1.0,
                            m_GrayscaleConverter.Minimum);
        }

        [Test]
        public void ImageSource_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(m_Model.ImageSource);
        }

        [Test]
        public void IsShowPheromones_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Model.IsShowPheromones);
        }

        [Test]
        public void IsShowPheromones_SendsMessage_ForNewValue()
        {
            // Arrange
            // Act
            m_Model.IsShowPheromones = true;

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <PheromonesModelsSetMessage>(x => x.IsShowPheromones));
        }

        [Test]
        public void IsShowPheromones_SetsValue_ForNewValue()
        {
            // Arrange
            // Act
            m_Model.IsShowPheromones = true;

            // Assert
            Assert.True(m_Model.IsShowPheromones);
        }

        [Test]
        public void Maximum_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Model.Maximum);
        }

        [Test]
        public void Minimum_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(string.Empty,
                            m_Model.Minimum);
        }

        [Test]
        public void PheromonesHandler_CallsDispatcher_WhenCalled()
        {
            // Arrange
            var message = new PheromonesModelChangedMessage();

            m_Dispatcher.ClearReceivedCalls();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            m_Dispatcher.Received().BeginInvoke(m_Model.Update);
        }

        [Test]
        public void PheromonesHandler_CallsGenerateImageSource_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Minimum.Returns(10.0);

            var message = new PheromonesModelChangedMessage();

            // Act
            m_Model.PheromonesHandler(message);

            // Assert
            Assert.AreEqual(m_GrayscaleConverter.Minimum,
                            m_PheromonesModel.Minimum);
        }

        [Test]
        public void Update_RaisesEvent_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     string.Empty);

            // Act
            m_Model.Update();

            // Assert
            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void Update_SetsAverage_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Average.Returns(1.0);

            // Act
            m_Model.Update();

            // Assert
            Assert.AreEqual("1",
                            m_Model.Average);
        }

        [Test]
        public void Update_SetsImageSource_WhenCalled()
        {
            // Arrange
            m_ImageSourceConverter.ImageSource.Returns(new BitmapImage());

            // Act
            m_Model.Update();

            // Assert
            Assert.AreEqual(m_ImageSourceConverter.ImageSource,
                            m_Model.ImageSource);
        }

        [Test]
        public void Update_SetsIsShowPheromones_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.IsShowPheromones.Returns(true);

            // Act
            m_Model.Update();

            // Assert
            Assert.True(m_Model.IsShowPheromones);
        }

        [Test]
        public void Update_SetsMaximum_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Maximum.Returns(1.0);

            // Act
            m_Model.Update();

            // Assert
            Assert.AreEqual("1",
                            m_Model.Maximum);
        }

        [Test]
        public void Update_SetsMinimum_WhenCalled()
        {
            // Arrange
            m_PheromonesModel.Minimum.Returns(1.0);

            // Act
            m_Model.Update();

            // Assert
            Assert.AreEqual("1",
                            m_Model.Minimum);
        }
    }
}