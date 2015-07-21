using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;

namespace Selkie.Framework.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackSettingsSourceManagerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();

            m_Sut = new RacetrackSettingsSourceManager(m_Logger,
                                                       m_Bus);
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private RacetrackSettingsSourceManager m_Sut;

        [Test]
        public void ColonyRacetrackSettingsRequestHandler_SendsMessage_WhenCalled()
        {
            var message = new ColonyRacetrackSettingsRequestMessage();

            m_Sut.ColonyRacetrackSettingsRequestHandler(message);

            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetrackSettingsChangedMessage>());
        }

        [Test]
        public void Constructor_SubscribesToColonyRacetrackSettingsRequestMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Func <ColonyRacetrackSettingsRequestMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribesToColonyRacetrackSettingsSetMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Func <ColonyRacetrackSettingsSetMessage, Task>>());
        }

        [Test]
        public void RacetrackSettingsSetMessageHandler_CreatesNewSource_WhenCalled()
        {
            var message = new ColonyRacetrackSettingsSetMessage
                          {
                              TurnRadius = 100.0,
                              IsPortTurnAllowed = true,
                              IsStarboardTurnAllowed = true
                          };

            m_Sut.ColonyRacetrackSettingsSetHandler(message);

            IRacetrackSettingsSource actual = m_Sut.Source;

            Assert.AreEqual(100.0,
                            actual.TurnRadius);
            Assert.True(actual.IsPortTurnAllowed);
            Assert.True(actual.IsStarboardTurnAllowed);
        }

        [Test]
        public void RacetrackSettingsSetMessageHandler_SendsMessage_WhenCalled()
        {
            var message = new ColonyRacetrackSettingsSetMessage
                          {
                              TurnRadius = 100.0,
                              IsPortTurnAllowed = true,
                              IsStarboardTurnAllowed = true
                          };

            m_Sut.ColonyRacetrackSettingsSetHandler(message);

            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetrackSettingsChangedMessage>());
        }

        [Test]
        public void Source_ReturnsDefault()
        {
            Assert.NotNull(m_Sut.Source);
        }
    }
}