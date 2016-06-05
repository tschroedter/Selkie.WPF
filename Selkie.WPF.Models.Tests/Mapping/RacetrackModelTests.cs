﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Converter = Substitute.For <IPathToRacetracksConverter>();

            m_Model = new RacetrackModel(m_Bus,
                                         m_Converter);
        }

        private IPathToRacetracksConverter m_Converter;
        private RacetrackModel m_Model;
        private ISelkieInMemoryBus m_Bus;

        private ColonyBestTrailMessage CreateBestTrailMessage()
        {
            var message = new ColonyBestTrailMessage
                          {
                              Iteration = 1,
                              Trail = new[]
                                      {
                                          0,
                                          1
                                      },
                              Length = 123.0,
                              Type = "Type",
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3
                          };

            return message;
        }

        [Test]
        public void BestTrailHandler_CallsUpdate_WhenCalled()
        {
            // Arrange
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            // Act
            m_Model.ColonyBestTrailHandler(message);

            // Assert
            Assert.True(m_Converter.Path.SequenceEqual(message.Trail));
        }

        [Test]
        public void ColonyLineResponsedHandler_CallsUpdate_WhenCalled()
        {
            // Arrange
            var message = new ColonyLineResponseMessage();

            // Act
            m_Model.ColonyLineResponsedHandler(message);

            // Assert
            m_Bus.Received()
                 .Publish(Arg.Any <RacetrackModelChangedMessage>());
        }

        [Test]
        public void ColonyLineResponsedHandler_ClearsRacetracks_WhenCalled()
        {
            // Arrange
            var message = new ColonyLineResponseMessage();

            // Act
            m_Model.ColonyLineResponsedHandler(message);

            // Assert
            m_Bus.Received()
                 .Publish(Arg.Any <RacetrackModelChangedMessage>());
        }

        [Test]
        public void Constructor_SubscribeToBestTrailMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ColonyBestTrailMessage>>());
        }

        [Test]
        public void Constructor_SubscribeToColonyLineResponseMessage_WhenCreated()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Action <ColonyLineResponseMessage>>());
        }

        [Test]
        public void Paths_ReturnsNotNull_WhenCalled()
        {
            Assert.NotNull(m_Model.Paths);
        }

        [Test]
        public void Update_CallsConvert_WhenCalled()
        {
            // Arrange
            var trail = new[]
                        {
                            0,
                            1
                        };

            // Act
            m_Model.Update(trail);

            // Assert
            m_Converter.Received().Convert();
        }

        [Test]
        public void Update_SetsConverterPath_WhenCalled()
        {
            // Arrange
            var trail = new[]
                        {
                            0,
                            1
                        };

            // Act
            m_Model.Update(trail);

            // Assert
            Assert.AreEqual(trail,
                            m_Converter.Path);
        }
    }
}