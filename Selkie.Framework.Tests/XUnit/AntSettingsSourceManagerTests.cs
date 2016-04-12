using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class AntSettingsSourceManagerTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyAntSettingsRequestMessage_WhenCalled(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] AntSettingsSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyAntSettingsRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyAntSettingsSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] AntSettingsSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyAntSettingsSetMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyLinesChangedMessage_WhenCalled(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull] AntSettingsSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyLinesChangedMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SendsMessage_WhenCalled(
            [NotNull] ISelkieInMemoryBus bus,
            [NotNull] IAntSettingsSourceFactory factory,
            [NotNull] IAntSettingsSource source)
        {
            // Arrange
            factory.Create(false,
                           0).ReturnsForAnyArgs(source);

            // Act
            // ReSharper disable once UnusedVariable
            var sut = new AntSettingsSourceManager(bus,
                                                   factory);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <ColonyAntSettingsResponseMessage>(x =>
                                                                       x.IsFixedStartNode == source.IsFixedStartNode &&
                                                                       x.FixedStartNode == source.FixedStartNode));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SetsSource_WhenCalled(
            [NotNull] AntSettingsSourceManager sut)
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(sut.Source);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyAntSettingsSetHandler_CallsFactory_WhenCalled(
            [NotNull] ISelkieInMemoryBus bus,
            [NotNull] IAntSettingsSourceFactory factory,
            [NotNull] IAntSettingsSource source,
            [NotNull] ColonyAntSettingsSetMessage message)
        {
            // Arrange
            factory.Create(false,
                           0).ReturnsForAnyArgs(source);

            var sut = new AntSettingsSourceManager(bus,
                                                   factory);

            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyAntSettingsSetHandler(message);

            // Assert
            factory.Received().Create(message.IsFixedStartNode,
                                      message.FixedStartNode);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesChangedHandler_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull, Frozen] IAntSettingsSource source,
            [NotNull] AntSettingsSourceManager sut,
            [NotNull] ColonyLinesChangedMessage message)
        {
            // Arrange
            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyLinesChangedHandler(message);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <ColonyAntSettingsResponseMessage>(x =>
                                                                       x.IsFixedStartNode == source.IsFixedStartNode &&
                                                                       x.FixedStartNode == source.FixedStartNode));
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesChangedHandler_CallsUpdateSource_WhenCalled(
            [NotNull, Frozen] IAntSettingsSourceFactory factory,
            [NotNull] AntSettingsSourceManager sut,
            [NotNull] ColonyLinesChangedMessage message)
        {
            // Arrange
            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyLinesChangedHandler(message);

            // Assert
            factory.Received()
                   .Create(Arg.Any <bool>(),
                           Arg.Any <int>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyAntSettingsSetHandler_CallsFactoryToReleaseOldSource_WhenCalled(
            [NotNull, Frozen] IAntSettingsSourceFactory factory,
            [NotNull, Frozen] IAntSettingsSource source,
            [NotNull] AntSettingsSourceManager sut,
            [NotNull] ColonyAntSettingsSetMessage message)
        {
            // Arrange
            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyAntSettingsSetHandler(message);

            // Assert
            factory.Received().Release(source);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyAntSettingsRequestHandler_SendsMessage_WhenCalled(
            [NotNull, Frozen] ISelkieInMemoryBus bus,
            [NotNull, Frozen] IAntSettingsSource source,
            [NotNull] ColonyAntSettingsRequestMessage message,
            [NotNull] AntSettingsSourceManager sut)
        {
            // Arrange
            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyAntSettingsRequestHandler(message);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <ColonyAntSettingsResponseMessage>(x =>
                                                                       x.IsFixedStartNode == source.IsFixedStartNode &&
                                                                       x.FixedStartNode == source.FixedStartNode));
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyAntSettingsSetHandler_CallsSendResponseMessage_WhenCalled(
            [NotNull] ISelkieInMemoryBus bus,
            [NotNull] IAntSettingsSourceFactory factory,
            [NotNull] IAntSettingsSource source,
            [NotNull] ColonyAntSettingsSetMessage message)
        {
            // Arrange
            factory.Create(false,
                           0).ReturnsForAnyArgs(source);

            var sut = new AntSettingsSourceManager(bus,
                                                   factory);

            // Act
            sut.ColonyAntSettingsSetHandler(message);

            // Assert
            bus.Received()
               .PublishAsync(Arg.Is <ColonyAntSettingsResponseMessage>(x =>
                                                                       x.IsFixedStartNode == source.IsFixedStartNode &&
                                                                       x.FixedStartNode == source.FixedStartNode));
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyAntSettingsSetHandler_SetsSource_WhenCalled(
            [NotNull] ISelkieInMemoryBus bus,
            [NotNull] IAntSettingsSourceFactory factory,
            [NotNull] IAntSettingsSource source,
            [NotNull] ColonyAntSettingsSetMessage message)
        {
            // Arrange
            factory.Create(false,
                           0).ReturnsForAnyArgs(source);

            var sut = new AntSettingsSourceManager(bus,
                                                   factory);

            // Act
            // ReSharper disable once UnusedVariable
            sut.ColonyAntSettingsSetHandler(message);

            // Assert
            Assert.Equal(source,
                         sut.Source);
        }
    }
}