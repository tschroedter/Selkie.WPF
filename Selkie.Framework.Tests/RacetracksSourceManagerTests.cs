using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.Windsor;

namespace Selkie.Framework.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetracksSourceManagerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieBus>();
            m_Converter = Substitute.For <IRacetracksDtoToRacetracksConverter>();

            m_Sut = new RacetracksSourceManager(m_Logger,
                                                m_Bus,
                                                m_Converter);
        }

        private IRacetracksDtoToRacetracksConverter m_Converter;
        private ISelkieLogger m_Logger;
        private ISelkieBus m_Bus;
        private RacetracksSourceManager m_Sut;

        private static Racetracks CreateRacetracks()
        {
            return new Racetracks
                   {
                       ForwardToForward = new[]
                                          {
                                              new[]
                                              {
                                                  Substitute.For <IPath>()
                                              }
                                          },
                       ForwardToReverse = new[]
                                          {
                                              new[]
                                              {
                                                  Substitute.For <IPath>()
                                              }
                                          },
                       ReverseToForward = new[]
                                          {
                                              new[]
                                              {
                                                  Substitute.For <IPath>()
                                              }
                                          },
                       ReverseToReverse = new[]
                                          {
                                              new[]
                                              {
                                                  Substitute.For <IPath>()
                                              }
                                          }
                   };
        }

        private RacetracksResponseMessage CreateRacetracksResponseMessage()
        {
            var message = new RacetracksResponseMessage
                          {
                              Racetracks = new RacetracksDto()
                          };

            return message;
        }

        [Test]
        public void Constructor_SendsRacetracksGetMessage_WhenCreated()
        {
            m_Bus.Received().PublishAsync(Arg.Any <RacetracksGetMessage>());
        }

        [Test]
        public void Constructor_SubscribeToColonyRacetracksRequestMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Action <ColonyRacetracksRequestMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToRacetracksResponseMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Action <RacetracksResponseMessage>>());
        }

        [Test]
        public void RacetracksChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();

            // Act
            m_Sut.RacetracksResponseHandler(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void RacetracksChangedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());

            // Act
            m_Sut.RacetracksResponseHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksResponseMessage>());
        }

        [Test]
        public void RacetracksChangedHandler_SetsDto_WhenCalled()
        {
            // Arrange
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();

            // Act
            m_Sut.RacetracksResponseHandler(message);

            // Assert
            Assert.AreEqual(message.Racetracks,
                            m_Converter.Dto);
        }

        [Test]
        public void RacetracksChangedHandler_SetsRacetracks_WhenCalled()
        {
            // Arrange
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();
            m_Converter.Racetracks.Returns(new Racetracks());

            // Act
            m_Sut.RacetracksResponseHandler(message);

            // Assert
            Assert.AreEqual(m_Converter.Racetracks,
                            m_Sut.Racetracks);
        }

        [Test]
        public void SendColonyRacetracksResponseMessage_CallsSendColonyRacetracksResponseMessage_WhenCalled()
        {
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksResponseHandler(message);

            // Act
            m_Sut.ColonyRacetracksGetHandler(new ColonyRacetracksRequestMessage());

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksResponseMessage>());
        }

        [Test]
        public void SendColonyRacetracksResponseMessage_DoesnNotSendMessage_ForRacetracksEmpty()
        {
            // Arrange
            Assert.False(m_Sut.Racetracks.ForwardToForward.Any());

            // Act
            m_Sut.SendColonyRacetracksResponseMessage();

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyRacetracksResponseMessage>());
        }

        [Test]
        public void SendColonyRacetracksResponseMessage_LogsRacetracks_ForRacetracks()
        {
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksResponseHandler(message);

            // Act
            m_Sut.SendColonyRacetracksResponseMessage();

            // Assert
            m_Logger.Received().Info("Racetracks");
        }

        [Test]
        public void SendColonyRacetracksResponseMessage_SendsMessage_ForRacetracks()
        {
            RacetracksResponseMessage message = CreateRacetracksResponseMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksResponseHandler(message);

            // Act
            m_Sut.SendColonyRacetracksResponseMessage();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksResponseMessage>());
        }
    }
}