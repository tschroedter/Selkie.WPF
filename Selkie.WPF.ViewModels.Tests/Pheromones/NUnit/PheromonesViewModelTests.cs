using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Pheromones;
using Selkie.WPF.ViewModels.Tests.NUnit;

namespace Selkie.WPF.ViewModels.Tests.Pheromones.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PheromonesViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Dispatcher = Substitute.For <IApplicationDispatcher>();
            m_PheromonesModel = Substitute.For <IPheromonesModel>();
            m_GrayscaleConverter = Substitute.For <IGrayscaleConverter>();
            m_ImageSourceConverter = Substitute.For <IBitmapSourceConverter>();

            m_Model = new PheromonesViewModel(m_Logger,
                                              m_Bus,
                                              m_Dispatcher,
                                              m_PheromonesModel,
                                              m_GrayscaleConverter,
                                              m_ImageSourceConverter);
        }

        private PheromonesViewModel m_Model;
        private ILogger m_Logger;
        private IBus m_Bus;
        private IApplicationDispatcher m_Dispatcher;
        private IPheromonesModel m_PheromonesModel;
        private IGrayscaleConverter m_GrayscaleConverter;
        private IBitmapSourceConverter m_ImageSourceConverter;

        [Test]
        public void Constructor_SubscribeToPheromonesModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <PheromonesModelChangedMessage, Task>>());
        }

        [Test]
        public void DefaultAverageTest()
        {
            Assert.AreEqual(string.Empty,
                            m_Model.Average);
        }

        [Test]
        public void DefaultImageSourceTest()
        {
            Assert.NotNull(m_Model.ImageSource);
        }

        [Test]
        public void DefaultMaximumTest()
        {
            Assert.AreEqual(string.Empty,
                            m_Model.Maximum);
        }

        [Test]
        public void DefaultMinimumTest()
        {
            Assert.AreEqual(string.Empty,
                            m_Model.Minimum);
        }

        [Test]
        public void GenerateImageSourceCallsConvertOnGrayscaleConverterTest()
        {
            m_GrayscaleConverter.ClearReceivedCalls();

            m_Model.GenerateImageSource();

            m_GrayscaleConverter.Received().Convert();
        }

        [Test]
        public void GenerateImageSourceCallsConvertOnImageSourceConverterTest()
        {
            m_ImageSourceConverter.ClearReceivedCalls();

            m_Model.GenerateImageSource();

            m_ImageSourceConverter.Received().Convert();
        }

        [Test]
        public void GenerateImageSourceSetsDataTest()
        {
            var values = new List <List <int>>
                         {
                             new List <int>()
                         };

            m_GrayscaleConverter.Grayscale.Returns(values);

            m_Model.GenerateImageSource();

            Assert.AreEqual(values,
                            m_ImageSourceConverter.Data);
        }

        [Test]
        public void GenerateImageSourceSetsMaximumTest()
        {
            m_PheromonesModel.Maximum.Returns(1.0);

            m_Model.GenerateImageSource();

            Assert.AreEqual(1.0,
                            m_GrayscaleConverter.Maximum);
        }

        [Test]
        public void GenerateImageSourceSetsMinimumTest()
        {
            m_PheromonesModel.Minimum.Returns(1.0);

            m_Model.GenerateImageSource();

            Assert.AreEqual(1.0,
                            m_GrayscaleConverter.Minimum);
        }

        [Test]
        public void GenerateImageSourceSetsValuesTest()
        {
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

            m_Model.GenerateImageSource();

            Assert.AreEqual(values,
                            m_GrayscaleConverter.Pheromones);
        }

        [Test]
        public void PheromonesHandlerCallsDispatcherTest()
        {
            var message = new PheromonesModelChangedMessage();

            m_Dispatcher.ClearReceivedCalls();

            m_Model.PheromonesHandler(message);

            m_Dispatcher.Received().BeginInvoke(m_Model.Update);
        }

        [Test]
        public void PheromonesHandlerCallsGenerateImageSourceTest()
        {
            m_PheromonesModel.Minimum.Returns(10.0);

            var message = new PheromonesModelChangedMessage();

            m_Model.PheromonesHandler(message);

            Assert.AreEqual(m_GrayscaleConverter.Minimum,
                            m_PheromonesModel.Minimum);
        }

        [Test]
        public void UpdateRaisesEventTest()
        {
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     string.Empty);

            m_Model.Update();

            Assert.True(test.IsExpectedNotified);
        }

        [Test]
        public void UpdateSetsAverageTest()
        {
            m_PheromonesModel.Average.Returns(1.0);

            m_Model.Update();

            Assert.AreEqual("1",
                            m_Model.Average);
        }

        [Test]
        public void UpdateSetsImageSourceTest()
        {
            m_ImageSourceConverter.ImageSource.Returns(new BitmapImage());

            m_Model.Update();

            Assert.AreEqual(m_ImageSourceConverter.ImageSource,
                            m_Model.ImageSource);
        }

        [Test]
        public void UpdateSetsMaximumTest()
        {
            m_PheromonesModel.Maximum.Returns(1.0);

            m_Model.Update();

            Assert.AreEqual("1",
                            m_Model.Maximum);
        }

        [Test]
        public void UpdateSetsMinimumTest()
        {
            m_PheromonesModel.Minimum.Returns(1.0);

            m_Model.Update();

            Assert.AreEqual("1",
                            m_Model.Minimum);
        }
    }
}