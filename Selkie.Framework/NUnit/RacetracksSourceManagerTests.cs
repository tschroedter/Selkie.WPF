using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converter;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;

namespace Selkie.Framework.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetracksSourceManagerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Converter = Substitute.For <IRacetracksDtoToRacetracksConverter>();

            m_Sut = new RacetracksSourceManager(m_Logger,
                                                m_Bus,
                                                m_Converter);
        }

        private IRacetracksDtoToRacetracksConverter m_Converter;
        private ILogger m_Logger;
        private IBus m_Bus;
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

        private RacetracksChangedMessage CreateRacetracksChangedMessage()
        {
            var message = new RacetracksChangedMessage
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
                                            Arg.Any <Func <ColonyRacetracksRequestMessage, Task>>());
        }

        [Test]
        public void Constructor_SubscribeToRacetracksChangedMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Sut.GetType().FullName,
                                            Arg.Any <Func <RacetracksChangedMessage, Task>>());
        }

        [Test]
        public void RacetracksChangedHandler_CallsConvert_WhenCalled()
        {
            // Arrange
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();

            // Act
            m_Sut.RacetracksChangedHandler(message);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void RacetracksChangedHandler_SendsMessage_WhenCalled()
        {
            // Arrange
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());

            // Act
            m_Sut.RacetracksChangedHandler(message);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksChangedMessage>());
        }

        [Test]
        public void RacetracksChangedHandler_SetsDto_WhenCalled()
        {
            // Arrange
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();

            // Act
            m_Sut.RacetracksChangedHandler(message);

            // Assert
            Assert.AreEqual(message.Racetracks,
                            m_Converter.Dto);
        }

        [Test]
        public void RacetracksChangedHandler_SetsRacetracks_WhenCalled()
        {
            // Arrange
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();
            m_Converter.Racetracks.Returns(new Racetracks());

            // Act
            m_Sut.RacetracksChangedHandler(message);

            // Assert
            Assert.AreEqual(m_Converter.Racetracks,
                            m_Sut.Racetracks);
        }

        [Test]
        public void SendColonyRacetracksChangedMessage_CallsSendColonyRacetracksChangedMessage_WhenCalled()
        {
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksChangedHandler(message);

            // Act
            m_Sut.ColonyRacetracksGetHandler(new ColonyRacetracksRequestMessage());

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksChangedMessage>());
        }

        [Test]
        public void SendColonyRacetracksChangedMessage_DoesnNotSendMessage_ForRacetracksEmpty()
        {
            // Arrange
            Assert.False(m_Sut.Racetracks.ForwardToForward.Any());

            // Act
            m_Sut.SendColonyRacetracksChangedMessage();

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <ColonyRacetracksChangedMessage>());
        }

        [Test]
        public void SendColonyRacetracksChangedMessage_LogsRacetracks_ForRacetracks()
        {
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksChangedHandler(message);

            // Act
            m_Sut.SendColonyRacetracksChangedMessage();

            // Assert
            m_Logger.Received().Info("Racetracks");
        }

        [Test]
        public void SendColonyRacetracksChangedMessage_SendsMessage_ForRacetracks()
        {
            RacetracksChangedMessage message = CreateRacetracksChangedMessage();
            m_Converter.Racetracks.Returns(CreateRacetracks());
            m_Sut.RacetracksChangedHandler(message);

            // Act
            m_Sut.SendColonyRacetracksChangedMessage();

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ColonyRacetracksChangedMessage>());
        }
    }
}