using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Selkie.Framework.Converters;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Services.Lines.Common.Dto;

namespace Selkie.Framework.Tests.Converters.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class TestLinesDtoToLinesConverterTests
    {
        private static void AssertLineForward(ILine actual)
        {
            AssertLine(actual,
                       1,
                       2.0,
                       3.0,
                       4.0,
                       5.0,
                       false,
                       Constants.LineDirection.Forward);
        }

        private static void AssertLineReverse(ILine actual)
        {
            AssertLine(actual,
                       10,
                       20.0,
                       30.0,
                       40.0,
                       50.0,
                       false,
                       Constants.LineDirection.Reverse);
        }

        private static void AssertLineUnknown(ILine actual)
        {
            AssertLine(actual,
                       100,
                       200.0,
                       300.0,
                       400.0,
                       500.0,
                       false,
                       Constants.LineDirection.Unknown);
        }

        private static void AssertLineNull(ILine actual)
        {
            AssertLine(actual,
                       1000,
                       2000.0,
                       3000.0,
                       4000.0,
                       5000.0,
                       false,
                       Constants.LineDirection.Unknown);
        }

        private static void AssertLine(ILine actual,
                                       int expectedId,
                                       double expectedX1,
                                       double expectedY1,
                                       double expectedX2,
                                       double expectedY2,
                                       bool isUnknown,
                                       Constants.LineDirection direction)
        {
            Assert.AreEqual(expectedId,
                            actual.Id,
                            "Id");
            Assert.AreEqual(expectedX1,
                            actual.X1,
                            "X1");
            Assert.AreEqual(expectedY1,
                            actual.Y1,
                            "Y1");
            Assert.AreEqual(expectedX2,
                            actual.X2,
                            "X2");
            Assert.AreEqual(expectedY2,
                            actual.Y2,
                            "Y2");
            Assert.AreEqual(isUnknown,
                            actual.IsUnknown,
                            "IsUnknown");
            Assert.AreEqual(direction,
                            actual.RunDirection,
                            "RunDirection");
        }

        private static LineDto[] CreateLineDtos()
        {
            return new[]
                   {
                       CreateLineDtoRunDirectionForward(),
                       CreateLineDtoRunDirectionReverse()
                   };
        }

        private static LineDto CreateLineDtoRunDirectionForward()
        {
            return new LineDto
                   {
                       Id = 1,
                       IsUnknown = false,
                       RunDirection = "Forward",
                       X1 = 2.0,
                       Y1 = 3.0,
                       X2 = 4.0,
                       Y2 = 5.0
                   };
        }

        private static LineDto CreateLineDtoRunDirectionReverse()
        {
            return new LineDto
                   {
                       Id = 10,
                       IsUnknown = false,
                       RunDirection = "Reverse",
                       X1 = 20.0,
                       Y1 = 30.0,
                       X2 = 40.0,
                       Y2 = 50.0
                   };
        }

        private static LineDto CreateLineDtoRunDirectionUnknown()
        {
            return new LineDto
                   {
                       Id = 100,
                       IsUnknown = false,
                       RunDirection = "Unknown",
                       X1 = 200.0,
                       Y1 = 300.0,
                       X2 = 400.0,
                       Y2 = 500.0
                   };
        }

        private static LineDto CreateLineDtoRunDirectionNull()
        {
            return new LineDto
                   {
                       Id = 1000,
                       IsUnknown = false,
                       RunDirection = null,
                       X1 = 2000.0,
                       Y1 = 3000.0,
                       X2 = 4000.0,
                       Y2 = 5000.0
                   };
        }

        [Test]
        public void Convert_SetsLines_WhenCalled()
        {
            // Arrange
            LineDto[] lineDtos = CreateLineDtos();
            var sut = new TestLinesDtoToLinesConverter
                      {
                          Dtos = lineDtos
                      };

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(2,
                            sut.Lines.Count());
        }

        [Test]
        public void CreateLine_ReturnsLine_ForLineDtoRunDirectionForward()
        {
            // Arrange
            LineDto lineDto = CreateLineDtoRunDirectionForward();
            var sut = new TestLinesDtoToLinesConverter();

            // Act
            ILine actual = sut.CreateLine(lineDto);

            // Assert
            AssertLineForward(actual);
        }

        [Test]
        public void CreateLine_ReturnsLine_ForLineDtoRunDirectionNull()
        {
            // Arrange
            LineDto lineDto = CreateLineDtoRunDirectionNull();
            var sut = new TestLinesDtoToLinesConverter();

            // Act
            ILine actual = sut.CreateLine(lineDto);

            // Assert
            AssertLineNull(actual);
        }

        [Test]
        public void CreateLine_ReturnsLine_ForLineDtoRunDirectionReverse()
        {
            // Arrange
            LineDto lineDto = CreateLineDtoRunDirectionReverse();
            var sut = new TestLinesDtoToLinesConverter();

            // Act
            ILine actual = sut.CreateLine(lineDto);

            // Assert
            AssertLineReverse(actual);
        }

        [Test]
        public void CreateLine_ReturnsLine_ForLineDtoRunDirectionUnknown()
        {
            // Arrange
            LineDto lineDto = CreateLineDtoRunDirectionUnknown();
            var sut = new TestLinesDtoToLinesConverter();

            // Act
            ILine actual = sut.CreateLine(lineDto);

            // Assert
            AssertLineUnknown(actual);
        }

        [Test]
        public void Dtos_Roundtrip()
        {
            // Arrange
            LineDto[] lineDtos = CreateLineDtos();

            // Act
            var sut = new TestLinesDtoToLinesConverter
                      {
                          Dtos = lineDtos
                      };

            // Assert
            Assert.NotNull(sut.Dtos);
            Assert.True(lineDtos.SequenceEqual(sut.Dtos));
        }
    }
}