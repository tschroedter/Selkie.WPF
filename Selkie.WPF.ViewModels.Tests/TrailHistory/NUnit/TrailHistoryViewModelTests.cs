using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Tests.NUnit;
using Selkie.WPF.ViewModels.TrailHistory;
using Selkie.WPF.ViewModels.TrailHistory.Converters;

namespace Selkie.WPF.ViewModels.Tests.TrailHistory.NUnit
{
    [TestFixture]
    internal sealed class TrailHistoryViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Model = Substitute.For <ITrailHistoryModel>();
            m_Detailses = new[]
                          {
                              Substitute.For <ITrailDetails>(),
                              Substitute.For <ITrailDetails>()
                          };

            m_Model.Trails.Returns(m_Detailses);
            m_Converter = Substitute.For <ITrailDetailsToDisplayHistoryRowsConverter>();

            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Logger = Substitute.For <ILogger>();
            m_Logger = Substitute.For <ILogger>();

            m_Sut = CreateSut(m_Logger,
                              m_Bus,
                              new TestImmediateDispatcher(),
                              m_Converter,
                              m_Model);
        }

        private ITrailHistoryModel m_Model;
        private ITrailDetails[] m_Detailses;
        private ITrailDetailsToDisplayHistoryRowsConverter m_Converter;
        private ILogger m_Logger;
        private IBus m_Bus;
        private TrailHistoryViewModel m_Sut;

        private IDisplayHistoryRow[] CreateDisplayHistoryRows()
        {
            var one = Substitute.For <IDisplayHistoryRow>();
            var two = Substitute.For <IDisplayHistoryRow>();


            return new[]
                   {
                       one,
                       two
                   };
        }

        private TrailHistoryViewModel CreateSut([NotNull] IApplicationDispatcher dispatcher)
        {
            TrailHistoryViewModel sut = CreateSut(Substitute.For <ILogger>(),
                                                  Substitute.For <IBus>(),
                                                  dispatcher,
                                                  Substitute.For <ITrailDetailsToDisplayHistoryRowsConverter>(),
                                                  Substitute.For <ITrailHistoryModel>());

            return sut;
        }

        private TrailHistoryViewModel CreateSut([NotNull] ILogger logger,
                                                [NotNull] IBus bus,
                                                [NotNull] IApplicationDispatcher dispatcher,
                                                [NotNull] ITrailDetailsToDisplayHistoryRowsConverter converter,
                                                [NotNull] ITrailHistoryModel model)
        {
            var sut = new TrailHistoryViewModel(logger,
                                                bus,
                                                dispatcher,
                                                converter,
                                                model);

            return sut;
        }

        [Test]
        public void Constructor_SubscribeToPheromonesModelChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Func <TrailHistoryModelChangedMessage, Task>>());
        }

        [Test]
        public void TrailHistoryModelChangedHandler_CallsBeginInvoke_WhenCalled()
        {
            // Arrange
            var message = new TrailHistoryModelChangedMessage();
            var dispatcher = Substitute.For <IApplicationDispatcher>();
            TrailHistoryViewModel sut = CreateSut(dispatcher);

            // Act
            sut.TrailHistoryModelChangedHandler(message);

            // Assert
            dispatcher.Received().BeginInvoke(Arg.Any <Action>());
        }

        [Test]
        public void TrailHistoryModelChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            var message = new TrailHistoryModelChangedMessage();

            // Act
            m_Sut.TrailHistoryModelChangedHandler(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void TrailHistoryModelChangedHandler_CallsUpdate_WhenCalled()
        {
            // Arrange
            var message = new TrailHistoryModelChangedMessage();

            // Act
            m_Sut.TrailHistoryModelChangedHandler(message);

            // Assert
            Assert.True(m_Converter.DisplayHistoryRows.SequenceEqual(m_Sut.Rows));
        }

        [Test]
        public void TrailHistoryModelChangedHandler_SetsTrails_WhenCalled()
        {
            // Arrange
            var message = new TrailHistoryModelChangedMessage();

            // Act
            m_Sut.TrailHistoryModelChangedHandler(message);

            // Assert
            Assert.True(m_Model.Trails.SequenceEqual(m_Converter.Trails));
        }

        [Test]
        public void Update_AddsRows_WhenCalled()
        {
            // Arrange
            IDisplayHistoryRow[] rows = CreateDisplayHistoryRows();

            // Act
            m_Sut.Update(rows);

            // Assert
            Assert.True(rows.SequenceEqual(m_Sut.Rows));
        }
    }
}