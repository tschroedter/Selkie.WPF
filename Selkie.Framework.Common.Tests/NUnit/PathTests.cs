using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Common.Tests.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PathTests
    {
        [TestFixture]
        internal sealed class CommonTests
        {
            [SetUp]
            public void Setup()
            {
                m_Segment1 = Substitute.For <IPolylineSegment>();
                m_Segment1.Length.Returns(1.0);

                m_Segment2 = Substitute.For <IPolylineSegment>();
                m_Segment2.Length.Returns(2.0);

                m_StartArcSegment = Substitute.For <ITurnCircleArcSegment>();
                m_Line = Substitute.For <ILine>();
                m_EndArcSegment = Substitute.For <ITurnCircleArcSegment>();
            }

            private ITurnCircleArcSegment m_StartArcSegment;
            private ILine m_Line;
            private ITurnCircleArcSegment m_EndArcSegment;
            private IPolylineSegment m_Segment1;
            private IPolylineSegment m_Segment2;

            [Test]
            public void Add_AddsSegment_ForSegment()
            {
                // Arrange
                // Act
                var sut = new Path(Point.Unknown);

                sut.Add(m_Segment1);

                // Assert
                Assert.AreEqual(1,
                                sut.Segments.Count(),
                                "Count");
                Assert.AreEqual(m_Segment1,
                                sut.Segments.First(),
                                "First");
                Assert.AreEqual(new Distance(1.0),
                                sut.Distance,
                                "Distance");
                Assert.False(sut.IsUnknown,
                             "IsUnknown");
            }

            [Test]
            public void Constructor_ReturnsInstance_ForOnePoint()
            {
                // Arrange
                var startPoint = new Point(10.0,
                                           10.0);

                // Act
                var sut = new Path(startPoint);

                // Assert
                Assert.AreEqual(startPoint,
                                sut.StartPoint,
                                "StartPoint");
                Assert.AreEqual(startPoint,
                                sut.EndPoint,
                                "EndPoint");
            }

            [Test]
            public void Constructor_ReturnsInstance_Polyline()
            {
                // Arrange
                var line = new Line(-10.0,
                                    -10.0,
                                    10.0,
                                    10.0);
                var polyline = new Polyline();
                polyline.AddSegment(line);

                // Act
                var sut = new Path(polyline);

                // Assert
                Assert.AreEqual(line.StartPoint,
                                sut.StartPoint,
                                "StartPoint");
                Assert.AreEqual(line.EndPoint,
                                sut.EndPoint,
                                "EndPoint");
                Assert.AreEqual(polyline,
                                sut.Polyline,
                                "Polyline");
            }

            [Test]
            public void IsUnknown_ReturnsTrue_ForUnknown()
            {
                // Arrange
                // Act
                Point sut = Point.Unknown;

                // Assert
                Assert.True(sut.IsUnknown);
            }

            [Test]
            public void Length_ReturnsThree_ForPathWithThreePoints()
            {
                // Arrange
                // Act
                var sut = new Path(m_Segment1.StartPoint);

                sut.Add(m_Segment1);
                sut.Add(m_Segment2);

                // Assert
                Assert.AreEqual(3.0,
                                sut.Distance.Length);
            }

            [Test]
            public void Reverse_DoesNothing_ForSinglePoint()
            {
                // Arrange
                var startPoint = new Point(-10.0,
                                           -10.0);

                // Act
                var sut = new Path(startPoint);

                IPath actual = sut.Reverse();

                // Assert
                Assert.AreEqual(startPoint,
                                actual.StartPoint,
                                "StartPoint");
                Assert.AreEqual(0,
                                actual.Segments.Count(),
                                "Count");
            }

            [Test]
            public void Reverse_ReversesSegmentsFirstSegment_ForPath()
            {
                // Arrange
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                // Act
                IPath reversed = sut.Reverse();

                IPolylineSegment[] segments = reversed.Segments.ToArray();
                IPolylineSegment segment = segments [ 0 ];

                // Assert
                Assert.AreEqual(m_EndArcSegment.StartPoint,
                                segment.StartPoint);
                Assert.AreEqual(m_EndArcSegment.EndPoint,
                                segment.EndPoint);
            }

            [Test]
            public void Reverse_ReversesSegmentsSecondSegment_ForPath()
            {
                // Arrange
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                // Act
                IPath reversed = sut.Reverse();

                IPolylineSegment[] segments = reversed.Segments.ToArray();
                IPolylineSegment segment = segments [ 1 ];

                // Assert
                Assert.AreEqual(m_Line.EndPoint,
                                segment.StartPoint);

                Assert.AreEqual(m_Line.StartPoint,
                                segment.EndPoint);
            }

            [Test]
            public void Reverse_ReversesSegmentsThirdSegment_ForPath()
            {
                // Arrange
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                // Act
                IPath reversed = sut.Reverse();

                IPolylineSegment[] segments = reversed.Segments.ToArray();
                IPolylineSegment segment = segments [ 2 ];

                // Assert
                Assert.AreEqual(m_StartArcSegment.EndPoint,
                                segment.StartPoint);
                Assert.AreEqual(m_StartArcSegment.StartPoint,
                                segment.EndPoint);
            }

            [Test]
            public void Segments_HasEndArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 2 ];

                // Assert
                Assert.AreEqual(m_EndArcSegment,
                                actual);
            }

            [Test]
            public void Segments_HasStartArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);


                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 0 ];

                // Assert
                Assert.AreEqual(m_StartArcSegment,
                                actual);
            }

            [Test]
            public void Segments_ReturnsLine_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 1 ];

                // Assert
                Assert.AreEqual(m_Line,
                                actual);
            }

            [Test]
            public void Segments_ReturnsZero_ForPoint()
            {
                // Arrange
                // Act
                var sut = new Path(m_Segment1.StartPoint);

                // Assert
                Assert.AreEqual(0,
                                sut.Segments.Count(),
                                "Count");
                Assert.AreEqual(new Distance(0.0),
                                sut.Distance,
                                "Distance");
            }

            [Test]
            public void ToString_ReturnsString()
            {
                // Arrange
                var line = new Line(-10.0,
                                    -10.0,
                                    10.0,
                                    10.0);
                var polyline = new Polyline();

                polyline.AddSegment(line);

                var sut = new Path(polyline);

                sut.Add(m_Segment1);

                const string expected = "Length: 29.28";

                // Act
                string actual = sut.ToString();

                // Assert
                Assert.AreEqual(expected,
                                actual);
            }
        }

        [TestFixture]
        internal sealed class ConstructorForPathTests
        {
            [SetUp]
            public void Setup()
            {
                m_Segment1 = Substitute.For <IPolylineSegment>();
                m_Segment1.Length.Returns(1.0);

                m_Segment2 = Substitute.For <IPolylineSegment>();
                m_Segment2.Length.Returns(2.0);

                m_StartArcSegment = Substitute.For <ITurnCircleArcSegment>();
                m_Line = Substitute.For <ILine>();
                m_EndArcSegment = Substitute.For <ITurnCircleArcSegment>();
            }

            private ITurnCircleArcSegment m_StartArcSegment;
            private ILine m_Line;
            private ITurnCircleArcSegment m_EndArcSegment;
            private IPolylineSegment m_Segment1;
            private IPolylineSegment m_Segment2;

            [Test]
            public void Segments_HasEndArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 2 ];

                // Assert
                Assert.AreEqual(m_EndArcSegment,
                                actual);
            }

            [Test]
            public void Segments_HasLine_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 1 ];

                // Assert
                Assert.AreEqual(m_Line,
                                actual);
            }

            [Test]
            public void Segments_HasStartArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_Line,
                                   m_EndArcSegment);


                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 0 ];

                // Assert
                Assert.AreEqual(m_StartArcSegment,
                                actual);
            }
        }

        [TestFixture]
        internal sealed class ConstructorForUTurnTests
        {
            [SetUp]
            public void Setup()
            {
                m_Segment1 = Substitute.For <IPolylineSegment>();
                m_Segment1.Length.Returns(1.0);

                m_Segment2 = Substitute.For <IPolylineSegment>();
                m_Segment2.Length.Returns(2.0);

                m_StartArcSegment = Substitute.For <ITurnCircleArcSegment>();
                m_EndArcSegment = Substitute.For <ITurnCircleArcSegment>();
                m_MiddleArcSegment = Substitute.For <ITurnCircleArcSegment>();
            }

            private ITurnCircleArcSegment m_StartArcSegment;
            private ITurnCircleArcSegment m_EndArcSegment;
            private IPolylineSegment m_Segment1;
            private IPolylineSegment m_Segment2;
            private ITurnCircleArcSegment m_MiddleArcSegment;

            [Test]
            public void Segments_HasEndArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_MiddleArcSegment,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 2 ];

                // Assert
                Assert.AreEqual(m_EndArcSegment,
                                actual);
            }

            [Test]
            public void Segments_HasMiddleArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_MiddleArcSegment,
                                   m_EndArcSegment);

                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 1 ];

                // Assert
                Assert.AreEqual(m_MiddleArcSegment,
                                actual);
            }

            [Test]
            public void Segments_HasStartArcSegment_ForPath()
            {
                // Arrange
                // Act
                var sut = new Path(m_StartArcSegment,
                                   m_MiddleArcSegment,
                                   m_EndArcSegment);


                IPolylineSegment[] segments = sut.Segments.ToArray();
                IPolylineSegment actual = segments [ 0 ];

                // Assert
                Assert.AreEqual(m_StartArcSegment,
                                actual);
            }
        }
    }
}