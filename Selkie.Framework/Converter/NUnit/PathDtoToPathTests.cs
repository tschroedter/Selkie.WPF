using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.Services.Racetracks.Common.Dto;

namespace Selkie.Framework.Converter.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class PathDtoToPathTests
    {
        private const double Tolerance = 0.1;

        private PathDto CreatePathDto()
        {
            PolylineDto polyline = CreatePolylineDto();
            return new PathDto
                   {
                       IsUnknown = false,
                       StartPoint = polyline.Segments.First().StartPoint,
                       EndPoint = polyline.Segments.Last().EndPoint,
                       Polyline = polyline
                   };
        }

        private PolylineDto CreatePolylineDto()
        {
            ArcSegmentDto startArcSegment = CreateArcSegmentDto();
            LineSegmentDto line = CreateLineSegmentDto();
            ArcSegmentDto endArcSegment = CreateArcSegmentDto();

            SegmentDto[] segments =
            {
                startArcSegment,
                line,
                endArcSegment
            };

            var dto = new PolylineDto
                      {
                          Segments = segments
                      };

            return dto;
        }

        private static void AssertTurnCircleArcSegment(ITurnCircleArcSegment actual,
                                                       ICircle expectedCircle,
                                                       Point expectedStartPoint,
                                                       Point expectedEndPoint,
                                                       Constants.CircleOrigin origin)
        {
            Assert.AreEqual(origin,
                            actual.CircleOrigin,
                            "CircleOrigin");
            Assert.AreEqual(expectedCircle.CentrePoint,
                            actual.CentrePoint,
                            "CentrePoint");
            Assert.AreEqual(expectedCircle.Radius,
                            actual.Radius,
                            "Radius");
            Assert.AreEqual(expectedStartPoint,
                            actual.StartPoint,
                            "StartPoint");
            Assert.AreEqual(expectedEndPoint,
                            actual.EndPoint,
                            "EndPoint");
        }

        private ArcSegmentDto CreateArcSegmentDto()
        {
            return new ArcSegmentDto
                   {
                       Circle = CreateCircleDto(),
                       TurnDirection = "Clockwise",
                       StartPoint = new PointDto
                                    {
                                        X = 1.0,
                                        Y = 5.0
                                    },
                       EndPoint = new PointDto
                                  {
                                      X = 1.0,
                                      Y = -1.0
                                  }
                   };
        }

        private LineSegmentDto CreateLineSegmentDto()
        {
            return new LineSegmentDto
                   {
                       RunDirection = "Forward",
                       IsUnknown = false,
                       StartPoint = new PointDto
                                    {
                                        X = 1.0,
                                        Y = 2.0
                                    },
                       EndPoint = new PointDto
                                  {
                                      X = 3.0,
                                      Y = 4.0
                                  }
                   };
        }

        private CircleDto CreateCircleDto()
        {
            return new CircleDto
                   {
                       CentrePoint = new PointDto
                                     {
                                         X = 1.0,
                                         Y = 2.0
                                     },
                       IsUnknown = false,
                       Radius = 3.0
                   };
        }

        private CircleDto CreateCircleDtoUnknown()
        {
            return new CircleDto
                   {
                       CentrePoint = new PointDto
                                     {
                                         X = 1.0,
                                         Y = 2.0
                                     },
                       IsUnknown = true,
                       Radius = 3.0
                   };
        }

        private ICircle CreateExpectedCircle()
        {
            return new Circle(1.0,
                              2.0,
                              3.0);
        }

        private PointDto CreatePointDto()
        {
            return new PointDto
                   {
                       X = 1.0,
                       Y = 2.0
                   };
        }

        private class UnknownDto : SegmentDto
        {
        }

        [Test]
        public void Convert_SetsPath_WhenCalled()
        {
            // Arrange
            PathDto dto = CreatePathDto();
            var sut = new PathDtoToPath
                      {
                          Dto = dto
                      };

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(3,
                            sut.Path.Segments.Count(),
                            "Count");
        }

        [Test]
        public void CreateCircleFormCircleDto_ReturnsCircle_ForDto()
        {
            // Arrange
            ICircle expected = CreateExpectedCircle();
            CircleDto dto = CreateCircleDto();
            var sut = new PathDtoToPath();

            // Act
            ICircle actual = sut.CreateCircleFormCircleDto(dto);

            // Assert
            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void CreateCircleFormCircleDto_ReturnsCircle_ForDtoIsUnnknownTrue()
        {
            // Arrange
            CircleDto dto = CreateCircleDtoUnknown();
            var sut = new PathDtoToPath();

            // Act
            ICircle actual = sut.CreateCircleFormCircleDto(dto);

            // Assert
            Assert.AreEqual(Circle.Unknown,
                            actual);
        }

        [Test]
        public void CreateLineDirectionFromString_ReturnsForward_ForForward()
        {
            // Arrange
            var sut = new PathDtoToPath();

            // Act
            Constants.LineDirection actual = sut.CreateLineDirectionFromString("Forward");

            // Assert
            Assert.AreEqual(Constants.LineDirection.Forward,
                            actual);
        }

        [Test]
        public void CreateLineDirectionFromString_Throws_ForUnknownString()
        {
            // Arrange
            var sut = new PathDtoToPath();

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.CreateLineDirectionFromString("abc"));
        }

        [Test]
        public void CreatePolyline_ReturnsAllSegements_ForPolylineDto()
        {
            // Arrange
            PolylineDto dto = CreatePolylineDto();
            var sut = new PathDtoToPath();

            // Act
            IPolyline actual = sut.CreatePolyline(dto);

            // Assert
            Assert.AreEqual(3,
                            actual.Segments.Count());
        }

        [Test]
        public void CreatePolyline_SetsFirstSegmentAsStart_ForPolylineDto()
        {
            // Arrange
            PolylineDto dto = CreatePolylineDto();
            var sut = new PathDtoToPath();

            // Act
            var actual = sut.CreatePolyline(dto).Segments.First() as ITurnCircleArcSegment;

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(Constants.CircleOrigin.Start,
                            actual.CircleOrigin);
        }

        [Test]
        public void CreatePolyline_SetsLastSegmentAsFinish_ForPolylineDto()
        {
            // Arrange
            PolylineDto dto = CreatePolylineDto();
            var sut = new PathDtoToPath();

            // Act
            var actual = sut.CreatePolyline(dto).Segments.Last() as ITurnCircleArcSegment;

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(Constants.CircleOrigin.Finish,
                            actual.CircleOrigin);
        }

        [Test]
        public void CreatePolylineSegmentFromDto_ReturnsLine_ForLineDto()
        {
            // Arrange
            var expected = new Line(0,
                                    new Point(1.0,
                                              2.0),
                                    new Point(3.0,
                                              4.0),
                                    Constants.LineDirection.Forward);

            LineSegmentDto dto = CreateLineSegmentDto();
            var sut = new PathDtoToPath();

            // Act
            var actual = sut.CreatePolylineSegmentFromDto(dto,
                                                          Constants.CircleOrigin.Start) as ILine;

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void CreatePolylineSegmentFromDto_ReturnsPolylineSegment_ForArcSegmentDtoCircleStart()
        {
            // Arrange
            ICircle expectedCircle = CreateExpectedCircle();
            var expectedStartPoint = new Point(1.0,
                                               5.0);
            var expectedEndPoint = new Point(1.0,
                                             -1.0);
            ArcSegmentDto dto = CreateArcSegmentDto();
            var sut = new PathDtoToPath();

            // Act
            var actual = sut.CreatePolylineSegmentFromDto(dto,
                                                          Constants.CircleOrigin.Start)
                         as ITurnCircleArcSegment;

            // Assert
            Assert.NotNull(actual);
            AssertTurnCircleArcSegment(actual,
                                       expectedCircle,
                                       expectedStartPoint,
                                       expectedEndPoint,
                                       Constants.CircleOrigin.Start);
        }

        [Test]
        public void CreatePolylineSegmentFromDto_ReturnsPolylineSegment_ForArcSegmentDtoFinish()
        {
            // Arrange
            ICircle expectedCircle = CreateExpectedCircle();
            var expectedStartPoint = new Point(1.0,
                                               5.0);
            var expectedEndPoint = new Point(1.0,
                                             -1.0);
            ArcSegmentDto dto = CreateArcSegmentDto();
            var sut = new PathDtoToPath();

            // Act
            var actual = sut.CreatePolylineSegmentFromDto(dto,
                                                          Constants.CircleOrigin.Finish)
                         as ITurnCircleArcSegment;

            // Assert
            Assert.NotNull(actual);
            AssertTurnCircleArcSegment(actual,
                                       expectedCircle,
                                       expectedStartPoint,
                                       expectedEndPoint,
                                       Constants.CircleOrigin.Finish);
        }

        [Test]
        public void CreatePolylineSegmentFromDto_Throws_ForUnknownDto()
        {
            // Arrange
            var dto = new UnknownDto();
            var sut = new PathDtoToPath();

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.CreatePolylineSegmentFromDto(dto,
                                                                                     Constants.CircleOrigin.Start));
        }

        [Test]
        public void CreateTurnDirectionFromString_ReturnsClockwise_ForClockwise()
        {
            // Arrange
            var sut = new PathDtoToPath();

            // Act
            Constants.TurnDirection actual = sut.CreateTurnDirectionFromString("Clockwise");

            // Assert
            Assert.AreEqual(Constants.TurnDirection.Clockwise,
                            actual);
        }

        [Test]
        public void CreateTurnDirectionFromString_Throws_ForUnknownString()
        {
            // Arrange
            var sut = new PathDtoToPath();

            // Act
            // Assert
            Assert.Throws <ArgumentException>(() => sut.CreateTurnDirectionFromString("abc"));
        }

        [Test]
        public void Dto_Roundtrip()
        {
            // Arrange
            PathDto dto = CreatePathDto();
            var sut = new PathDtoToPath
                      {
                          Dto = dto
                      };

            // Act
            // Assert
            Assert.AreEqual(dto,
                            sut.Dto);
        }

        [Test]
        public void FunctionUnderTest_ExpectedResult_UnderCondition()
        {
            // Arrange
            PointDto dto = CreatePointDto();
            var sut = new PathDtoToPath();

            // Act
            Point actual = sut.CreatePointFromPointDto(dto);

            // Assert
            Assert.True(Math.Abs(1.0 - actual.X) < Tolerance,
                        "X");
            Assert.True(Math.Abs(2.0 - actual.Y) < Tolerance,
                        "Y");
        }

        [Test]
        public void Path_Roundtrip()
        {
            // Arrange
            var path = Substitute.For <IPath>();
            var sut = new PathDtoToPath
                      {
                          Path = path
                      };

            // Act
            // Assert
            Assert.AreEqual(path,
                            sut.Path);
        }
    }
}