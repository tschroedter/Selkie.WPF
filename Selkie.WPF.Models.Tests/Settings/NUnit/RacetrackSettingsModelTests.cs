using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Settings;

namespace Selkie.WPF.Models.Tests.Settings.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackSettingsModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_MemoryBus = Substitute.For <ISelkieInMemoryBus>();
            m_Source = new RacetrackSettingsSource(300.0,
                                                   true,
                                                   true);

            m_Manager = Substitute.For <IRacetrackSettingsSourceManager>();
            m_Manager.Source.Returns(m_Source);

            m_Model = new RacetrackSettingsModel(m_Bus,
                                                 m_MemoryBus,
                                                 m_Manager);
        }

        private const double Tolerance = 0.1;
        private RacetrackSettingsModel m_Model;
        private IRacetrackSettingsSourceManager m_Manager;
        private RacetrackSettingsSource m_Source;
        private ISelkieBus m_Bus;
        private ISelkieInMemoryBus m_MemoryBus;

        [Test]
        public void ColonyCostMatrixChangedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new ColonyCostMatrixChangedMessage();

            // Act
            m_Model.ColonyCostMatrixChangedHandler(message);

            // Assert
            m_MemoryBus.Received()
                       .PublishAsync(Arg.Is <RacetrackSettingsChangedMessage>(x =>
                                                                              Math.Abs(x.TurnRadius -
                                                                                       m_Source.TurnRadius) <
                                                                              Tolerance &&
                                                                              x.IsPortTurnAllowed ==
                                                                              m_Source.IsPortTurnAllowed &&
                                                                              x.IsStarboardTurnAllowed ==
                                                                              m_Source.IsStarboardTurnAllowed));
        }

        [Test]
        public void ConstructorSubscribesToColonyCostMatrixChangedMessageTest()
        {
            m_Bus.Received()
                 .SubscribeAsync(m_Model.GetType().ToString(),
                                 Arg.Any <Action <ColonyCostMatrixChangedMessage>>());
        }

        [Test]
        public void ConstructorSubscribesToRacetrackSettingsRequestMessageTest()
        {
            m_MemoryBus.Received()
                       .SubscribeAsync(m_Model.GetType().ToString(),
                                       Arg.Any <Action <RacetrackSettingsRequestMessage>>());
        }

        [Test]
        public void ConstructorSubscribesToRacetrackSettingsSetMessageTest()
        {
            m_MemoryBus.Received()
                       .SubscribeAsync(m_Model.GetType().ToString(),
                                       Arg.Any <Action <RacetrackSettingsSetMessage>>());
        }

        [Test]
        public void IsPortTurnAllowed_ReturnsDefault()
        {
            Assert.True(m_Model.IsPortTurnAllowed);
        }

        [Test]
        public void IsStarboardTurnAllowed_ReturnsDefault()
        {
            Assert.True(m_Model.IsStarboardTurnAllowed);
        }

        [Test]
        public void RacetrackSettingsRequestHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new RacetrackSettingsRequestMessage();

            // Act
            m_Model.RacetrackSettingsRequestHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetrackSettingsRequestMessage>());
        }

        [Test]
        public void RacetrackSettingsSetHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            var message = new RacetrackSettingsSetMessage
                          {
                              TurnRadius = 1.0,
                              IsPortTurnAllowed = true,
                              IsStarboardTurnAllowed = true
                          };

            // Act
            m_Model.RacetrackSettingsSetHandler(message);

            // Assert
            m_Bus.Received()
                 .PublishAsync(
                               Arg.Is <ColonyRacetrackSettingsSetMessage>(
                                                                          x =>
                                                                          Math.Abs(x.TurnRadius - 1.0) < Tolerance &&
                                                                          x.IsPortTurnAllowed &&
                                                                          x.IsStarboardTurnAllowed));
        }

        [Test]
        public void TurnRadius_ReturnsDefault()
        {
            Assert.AreEqual(300.0,
                            m_Model.TurnRadius);
        }
    }
}