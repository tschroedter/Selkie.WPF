using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Selkie.EasyNetQ;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Services.Common.Dto;
using Selkie.Services.Racetracks.Common.Messages;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Framework.Tests.XUnit
{
    public sealed class CalculateCostMatrixManagerTests
    {
        private const double Tolerance = 0.1;

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SendsRacetrackSettingsSetMessage_WhenCalled(
            [NotNull] IRacetrackSettingsSource source,
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] IRacetrackSettingsSourceManager manager,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            source.TurnRadiusForPort.Returns(1.0);
            source.TurnRadiusForStarboard.Returns(2.0);
            manager.Source.Returns(source);
            bus.ClearReceivedCalls();

            // Act
            sut.Calculate();

            // Assert
            bus.Received()
               .PublishAsync(
                             Arg.Is <RacetrackSettingsSetMessage>(
                                                                  x =>
                                                                  Math.Abs(x.TurnRadiusForPort - 1.0) < 0.01 &&
                                                                  Math.Abs(x.TurnRadiusForStarboard - 2.0) < 0.01 &&
                                                                  x.IsPortTurnAllowed ==
                                                                  manager.Source.IsPortTurnAllowed &&
                                                                  x.IsStarboardTurnAllowed ==
                                                                  manager.Source.IsStarboardTurnAllowed));
        }

        [Theory]
        [AutoNSubstituteData]
        public void Calculate_SendsLinesSetMessage_WhenCalled(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull] CalculateCostMatrixManager sut)
        {
            // Arrange
            // Act
            sut.Calculate();

            // Assert
            bus.Received().PublishAsync(Arg.Is <LinesSetMessage>(x => x.LineDtos.Any()));
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDtos_ForLines(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            var lineOne = new Line(0,
                                   1.0,
                                   2.0,
                                   3.0,
                                   4.0);
            var lineTwo = new Line(1,
                                   5.0,
                                   6.0,
                                   7.0,
                                   8.0);

            // Act
            IEnumerable <LineDto> actual = sut.CreateLineDtos(new[]
                                                              {
                                                                  lineOne,
                                                                  lineTwo
                                                              });

            // Assert
            Assert.True(actual.Count() == 2);
        }

        [Theory]
        [AutoNSubstituteData]
        public void CreateLineDtos_ReturnsDto_ForLine(
            [NotNull, Frozen] ISelkieBus bus,
            [NotNull, Frozen] CalculateCostMatrixManager sut)
        {
            // Arrange
            var line = new Line(0,
                                1.0,
                                2.0,
                                3.0,
                                4.0);

            // Act
            LineDto actual = sut.CreateLineDtos(new[]
                                                {
                                                    line
                                                }).First();

            // Assert
            Assert.True(actual.Id == line.Id,
                        "Id");
            Assert.True(actual.IsUnknown == line.IsUnknown,
                        "IsUnknown");
            Assert.True(actual.RunDirection == line.RunDirection.ToString(),
                        "RunDirection");
            Assert.True(Math.Abs(actual.X1 - line.X1) < Tolerance,
                        "X1");
            Assert.True(Math.Abs(actual.Y1 - line.Y1) < Tolerance,
                        "Y1");
            Assert.True(Math.Abs(actual.X2 - line.X2) < Tolerance,
                        "X2");
            Assert.True(Math.Abs(actual.Y2 - line.Y2) < Tolerance,
                        "Y2");
        }
    }
}