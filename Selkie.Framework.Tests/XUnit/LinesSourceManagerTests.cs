using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using NSubstitute.Core;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Messages;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Services.Lines.Common;
using Selkie.Services.Lines.Common.Dto;
using Selkie.Services.Lines.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    [ExcludeFromCodeCoverage]
    public sealed class LinesSourceManagerTests
    {
        private const double Tolerance = 0.01;

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SendsRequestMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                               [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            // ReSharper disable once UnusedVariable
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);
            // Act
            // Assert
            bus.Received().PublishAsync(Arg.Any <TestLineRequestMessage>());
        }

        private static LinesSourceManager CreateManager(ISelkieLogger logger,
                                                        ISelkieBus bus)
        {
            var factory = Substitute.For <ILinesSourceFactory>();
            factory.Create(Arg.Any <IEnumerable <ILine>>()).Returns(CreateLinesSource);

            var converter = new TestLinesDtoToLinesConverter();

            return new LinesSourceManager(logger,
                                          bus,
                                          factory,
                                          converter);
        }

        private static ILinesSource CreateLinesSource(CallInfo arg)
        {
            return new LinesSource(new LinesToCostPerLineConverter(),
                                   ( IEnumerable <ILine> ) arg [ 0 ]);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToTestLineResponseMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                               [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <TestLineResponseMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyLinesRequestMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                                 [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyLinesRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyTestLinesRequestMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                                     [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyTestLinesRequestMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void Constructor_SubscribesToColonyTestLineSetMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                                [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);
            // Act
            // Assert
            bus.Received().SubscribeAsync(sut.GetType().FullName,
                                          Arg.Any <Action <ColonyTestLineSetMessage>>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void TestLineResponseHandler_UpdatesLines_WhenCalled([NotNull] ISelkieLogger logger,
                                                                    [NotNull, Frozen] ISelkieBus bus,
                                                                    [NotNull] TestLineResponseMessage message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.TestLineResponseHandler(message);

            // Assert
            Assert.True(message.LineDtos.Length == sut.Lines.Count());
        }

        [Theory]
        [AutoNSubstituteData]
        public void TestLineResponseHandler_ConvertsDtoIntoLines_WhenCalled([NotNull] ISelkieLogger logger,
                                                                            [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            TestLineResponseMessage message = CreateMessage();

            // Act
            sut.TestLineResponseHandler(message);

            // Assert
            AssertDtosEqualLines(message.LineDtos,
                                 sut.Lines);
        }

        [Theory]
        [AutoNSubstituteData]
        public void TestLineResponseHandler_SendsLinesSourceChangedMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                                      [NotNull, Frozen] ISelkieBus bus,
                                                                                      [NotNull] TestLineResponseMessage
                                                                                          message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.TestLineResponseHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <LinesSourceChangedMessage>(x => x.Lines.Count() == sut.Lines.Count()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void TestLineResponseHandler_SendsColonyLinesChangedMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                                      [NotNull, Frozen] ISelkieBus bus,
                                                                                      [NotNull] TestLineResponseMessage
                                                                                          message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.TestLineResponseHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyLinesChangedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendColonyLinesChangedMessage_UpdatesLines_WhenCalled([NotNull] ISelkieLogger logger,
                                                                          [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.SendColonyLinesChangedMessage();

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyLinesChangedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void SendColonyLinesChangedMessage_LogsLines_WhenCalled([NotNull] ISelkieLogger logger,
                                                                       [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.SendColonyLinesChangedMessage();

            // Assert
            logger.Received().Info(Arg.Is <string>(x => x.StartsWith("Lines")));
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyLinesRequestHandler_UpdatesLines_WhenCalled([NotNull] ISelkieLogger logger,
                                                                      [NotNull, Frozen] ISelkieBus bus,
                                                                      [NotNull] ColonyLinesRequestMessage message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.ColonyLinesRequestHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ColonyLinesChangedMessage>());
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyTestLineSetHandler_SendsMessage_ForKnownType([NotNull] ISelkieLogger logger,
                                                                       [NotNull, Frozen] ISelkieBus bus,
                                                                       [NotNull] ColonyTestLineSetMessage message)
        {
            // Arrange
            var type = TestLineType.Type.Create45DegreeLines;
            message.Type = type.ToString();

            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.ColonyTestLineSetHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <TestLineRequestMessage>(x => x.Types.Contains(type)));
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyTestLineSetHandler_Throws_ForUnknownType([NotNull] ISelkieLogger logger,
                                                                   [NotNull, Frozen] ISelkieBus bus,
                                                                   [NotNull] ColonyTestLineSetMessage message)
        {
            // Arrange
            message.Type = "Unknown";

            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.ColonyTestLineSetHandler(message));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CostPerLine_ReturnsCostPerLine([NotNull] ISelkieLogger logger,
                                                   [NotNull, Frozen] ISelkieBus bus,
                                                   [NotNull] TestLineResponseMessage message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            sut.TestLineResponseHandler(message);

            // Act
            // Assert
            Assert.True(sut.CostPerLine.Any());
        }

        [Theory]
        [AutoNSubstituteData]
        public void ColonyTestLinesRequestHandler_SendsMessage_WhenCalled([NotNull] ISelkieLogger logger,
                                                                          [NotNull, Frozen] ISelkieBus bus,
                                                                          [NotNull] ColonyTestLinesRequestMessage
                                                                              message)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            sut.ColonyTestLinesRequestHandler(message);

            // Assert
            bus.Received().PublishAsync(Arg.Is <ColonyTestLinesResponseMessage>(x => x.Types.Any()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void GetTestLineTypes_ReturnsTestLineTypesAsStrings_WhenCalled([NotNull] ISelkieLogger logger,
                                                                              [NotNull, Frozen] ISelkieBus bus)
        {
            // Arrange
            LinesSourceManager sut = CreateManager(logger,
                                                   bus);

            // Act
            IEnumerable <string> actual = sut.GetTestLineTypes();

            // Assert
            Assert.True(actual.Count() == 16);
        }

        private TestLineResponseMessage CreateMessage()
        {
            var dtos = new List <LineDto>
                       {
                           CreateLineDto(0,
                                         Constants.LineDirection.Forward),
                           CreateLineDto(1,
                                         Constants.LineDirection.Reverse),
                           CreateLineDto(2,
                                         Constants.LineDirection.Unknown)
                       };

            var message = new TestLineResponseMessage
                          {
                              LineDtos = dtos.ToArray()
                          };

            return message;
        }

        private LineDto CreateLineDto(int id,
                                      Constants.LineDirection lineDirection)
        {
            LineDto dto = CreateLineDto(id,
                                        lineDirection.ToString(),
                                        false);

            return dto;
        }

        private static LineDto CreateLineDto(int id,
                                             string runDirection,
                                             bool isUnknown)
        {
            var dto = new LineDto
                      {
                          Id = id,
                          X1 = 1.0 + id,
                          Y1 = 2.0 + id,
                          X2 = 3.0 + id,
                          Y2 = 4.0 + id,
                          RunDirection = runDirection,
                          IsUnknown = isUnknown
                      };

            return dto;
        }

        private void AssertDtosEqualLines(LineDto[] lineDtos,
                                          IEnumerable <ILine> lines)
        {
            foreach ( ILine line in lines )
            {
                LineDto dto = lineDtos.FirstOrDefault(x => x.Id == line.Id);

                Assert.NotNull(dto);

                AssertDtoEqualsLine(dto,
                                    line);
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void AssertDtoEqualsLine(LineDto dto,
                                         ILine line)
        {
            Assert.True(dto.Id == line.Id,
                        "Id - Expected: {0} Actual: {1}".Inject(dto.Id,
                                                                line.Id));
            Assert.True(Math.Abs(dto.X1 - line.X1) < Tolerance,
                        "X1 - Expected: {0} Actual: {1}".Inject(dto.X1,
                                                                line.X1));
            Assert.True(Math.Abs(dto.Y1 - line.Y1) < Tolerance,
                        "Y1 - Expected: {0} Actual: {1}".Inject(dto.Y1,
                                                                line.Y1));
            Assert.True(Math.Abs(dto.X2 - line.X2) < Tolerance,
                        "X2 - Expected: {0} Actual: {1}".Inject(dto.X2,
                                                                line.X2));
            Assert.True(Math.Abs(dto.Y2 - line.Y2) < Tolerance,
                        "Y2 - Expected: {0} Actual: {1}".Inject(dto.Y2,
                                                                line.Y2));
            Assert.True(dto.RunDirection == line.RunDirection.ToString(),
                        "RunDirection - Expected: {0} Actual: {1}".Inject(dto.RunDirection,
                                                                          line.RunDirection));
            Assert.True(dto.IsUnknown == line.IsUnknown,
                        "IsUnknown - Expected: {0} Actual: {1}".Inject(dto.IsUnknown,
                                                                       line.IsUnknown));
        }
    }
}