using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Converters;
using Selkie.WPF.Common.Interfaces.Converters;
using ArcSegment = System.Windows.Media.ArcSegment;
using Point = System.Windows.Point;

namespace Selkie.WPF.Converters.Tests.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class PathSegmentHelperTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = Substitute.For <IGeometryPointToWindowsPointConverter>();
            m_Point = new Point(100.0,
                                200.0);
            m_Converter.Point.Returns(m_Point);

            m_Helper = new PathSegmentHelper(m_Converter);
        }

        private IGeometryPointToWindowsPointConverter m_Converter;
        private PathSegmentHelper m_Helper;
        private Point m_Point;

        [Test]
        public void CalculateIsLargeArcReturnsFalseForClockwiseAndStartTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(0.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Clockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            Assert.False(m_Helper.CalculateIsLargeArc(segment));
        }

        [Test]
        public void CalculateIsLargeArcReturnsFalseForCounterclockwiseAndStartTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(10.0,
                                                     20.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Counterclockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            Assert.False(m_Helper.CalculateIsLargeArc(segment));
        }

        [Test]
        public void CalculateIsLargeArcReturnsFalseForCounterClockwiseAndStartTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(0.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Counterclockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            Assert.False(m_Helper.CalculateIsLargeArc(segment));
        }

        [Test]
        public void CalculateIsLargeArcReturnsTrueForClockwiseAndStartTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(10.0,
                                                     20.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Clockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            Assert.True(m_Helper.CalculateIsLargeArc(segment));
        }

        [Test]
        public void CalculateIsLargeArcReturnsTrueForCounterclockwiseAndStartTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(10.0,
                                                       20.0);
            var endPoint = new Geometry.Shapes.Point(20.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Counterclockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            Assert.True(m_Helper.CalculateIsLargeArc(segment));
        }

        [Test]
        public void PointRelativeToOriginTest()
        {
            var point = new Geometry.Shapes.Point(10.0,
                                                  10.0);

            Point actual = m_Helper.PointRelativeToOrigin(point);

            Assert.AreEqual(m_Point,
                            actual);
        }

        [Test]
        public void SegmentToArcSegmentForIsLargArcIsTrueTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(0.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Clockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            ArcSegment actual = m_Helper.SegmentToArcSegment(segment);

            Assert.AreEqual(m_Point,
                            actual.Point,
                            "Point");
            Assert.AreEqual(new Size(10.0,
                                     10.0),
                            actual.Size,
                            "Size");
            Assert.AreEqual(SweepDirection.Clockwise,
                            actual.SweepDirection,
                            "SweepDirection");
            Assert.False(actual.IsLargeArc,
                         "IsLargeArc");
        }

        [Test]
        public void SegmentToArcSegmentForTurnDirectionClockwiseTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(0.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Clockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            ArcSegment actual = m_Helper.SegmentToArcSegment(segment);

            Assert.AreEqual(m_Point,
                            actual.Point,
                            "Point");
            Assert.AreEqual(new Size(10.0,
                                     10.0),
                            actual.Size,
                            "Size");
            Assert.AreEqual(SweepDirection.Clockwise,
                            actual.SweepDirection,
                            "SweepDirection");
            Assert.False(actual.IsLargeArc,
                         "IsLargeArc");
        }

        [Test]
        public void SegmentToArcSegmentForTurnDirectionCounterClockwiseCaseOneTest()
        {
            var helper = new PathSegmentHelper(new GeometryPointToWindowsPointConverter());

            var circle = new Circle(new Geometry.Shapes.Point(350.0,
                                                              490.0),
                                    30.0);
            var startPoint = new Geometry.Shapes.Point(327.639320225002,
                                                       510.0);
            var endPoint = new Geometry.Shapes.Point(350.0,
                                                     520.0);


            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Counterclockwise,
                                                   Constants.CircleOrigin.Finish,
                                                   startPoint,
                                                   endPoint);

            ArcSegment actual = helper.SegmentToArcSegment(segment);

            Assert.AreEqual(new Point(400,
                                      1430),
                            actual.Point,
                            "Point");
            Assert.AreEqual(new Size(30.0,
                                     30.0),
                            actual.Size,
                            "Size");
            Assert.AreEqual(SweepDirection.Counterclockwise,
                            actual.SweepDirection,
                            "SweepDirection");
            Assert.True(actual.IsLargeArc,
                        "IsLargeArc");
        }

        [Test]
        public void SegmentToArcSegmentForTurnDirectionCounterClockwiseTest()
        {
            var circle = new Circle(10.0,
                                    10.0,
                                    10);
            var startPoint = new Geometry.Shapes.Point(20.0,
                                                       10.0);
            var endPoint = new Geometry.Shapes.Point(0.0,
                                                     10.0);

            var segment = new TurnCircleArcSegment(circle,
                                                   Constants.TurnDirection.Counterclockwise,
                                                   Constants.CircleOrigin.Start,
                                                   startPoint,
                                                   endPoint);

            ArcSegment actual = m_Helper.SegmentToArcSegment(segment);

            Assert.AreEqual(m_Point,
                            actual.Point,
                            "Point");
            Assert.AreEqual(new Size(10.0,
                                     10.0),
                            actual.Size,
                            "Size");
            Assert.AreEqual(SweepDirection.Counterclockwise,
                            actual.SweepDirection,
                            "SweepDirection");
            Assert.False(actual.IsLargeArc,
                         "IsLargeArc");
        }

        [Test]
        public void SegmentToLineSegmentTest()
        {
            var segment = new Line(10.0,
                                   10.0,
                                   20.0,
                                   20.0);

            LineSegment actual = m_Helper.SegmentToLineSegment(segment);

            Assert.AreEqual(m_Point,
                            actual.Point,
                            "Point");
            Assert.True(actual.IsStroked,
                        "IsStroked");
        }
    }
}