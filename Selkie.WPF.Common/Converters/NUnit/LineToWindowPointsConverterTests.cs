using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common.Converters.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class LineToWindowPointsConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_StartPoint = new Point(-10.0,
                                     -10.0);
            m_EndPoint = new Point(10.0,
                                   10.0);
            m_LineDirection = Constants.LineDirection.Forward;

            m_Line = new Line(m_StartPoint,
                              m_EndPoint);

            m_WindowsStartPoint = new System.Windows.Point(100.0,
                                                           200.0);
            m_WindowsEndPoint = new System.Windows.Point(300.0,
                                                         400.0);

            m_GeometryPointToWindowsPointConverter = Substitute.For <IGeometryPointToWindowsPointConverter>();
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint,
                                                                 m_WindowsEndPoint);

            m_Sut = new LineToWindowPointsConverter(m_GeometryPointToWindowsPointConverter)
                    {
                        Line = m_Line
                    };
            m_Sut.Line = m_Line;
            m_Sut.LineDirection = m_LineDirection;
            m_Sut.Convert();
        }

        private LineToWindowPointsConverter m_Sut;
        private Line m_Line;
        private Point m_StartPoint;
        private Point m_EndPoint;
        private Constants.LineDirection m_LineDirection;
        private IGeometryPointToWindowsPointConverter m_GeometryPointToWindowsPointConverter;
        private System.Windows.Point m_WindowsEndPoint;
        private System.Windows.Point m_WindowsStartPoint;

        [Test]
        public void ConvertPoints_CallsConverter_WhenCalled()
        {
            m_GeometryPointToWindowsPointConverter.ClearReceivedCalls();

            m_Sut.ConvertPoints(m_StartPoint,
                                m_EndPoint);

            m_GeometryPointToWindowsPointConverter.Received(2).Convert();
        }

        [Test]
        public void ConvertPoints_ReturnsFirstPoint_WhenCalled()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint);

            System.Windows.Point[] actual = m_Sut.ConvertPoints(m_StartPoint,
                                                                m_EndPoint).ToArray();

            System.Windows.Point point = actual [ 0 ];

            Assert.AreEqual(m_WindowsStartPoint,
                            point);
        }

        [Test]
        public void ConvertPoints_ReturnsSecondPoint_WhenCalled()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Sut.ConvertPoints(m_StartPoint,
                                                                m_EndPoint).ToArray();

            System.Windows.Point point = actual [ 1 ];

            Assert.AreEqual(m_WindowsEndPoint,
                            point);
        }

        [Test]
        public void ConvertPoints_ReturnsTwoPoints_WhenCalled()
        {
            System.Windows.Point[] actual = m_Sut.ConvertPoints(m_StartPoint,
                                                                m_EndPoint).ToArray();

            Assert.AreEqual(2,
                            actual.Length);
        }

        [Test]
        public void CreatePoints_ReturnsPoints_ForLineForward()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint,
                                                                 m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Sut.CreatePointsForLine(m_Line,
                                                                      Constants.LineDirection.Forward).ToArray();

            Assert.AreEqual(m_WindowsStartPoint,
                            actual [ 0 ],
                            "Point 1");
            Assert.AreEqual(m_WindowsEndPoint,
                            actual [ 1 ],
                            "Point 2");
        }

        [Test]
        public void CreatePoints_ReturnsPoints_ForLineReverse()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint,
                                                                 m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Sut.CreatePointsForLine(m_Line,
                                                                      Constants.LineDirection.Reverse).ToArray();

            Assert.AreEqual(m_WindowsStartPoint,
                            actual [ 0 ],
                            "Point 1");
            Assert.AreEqual(m_WindowsEndPoint,
                            actual [ 1 ],
                            "Point 2");
        }

        [Test]
        public void Line_ReturnsDefault()
        {
            Assert.AreEqual(m_Line,
                            m_Sut.Line);
        }

        [Test]
        public void Line_Roundtrip()
        {
            var other = new Line(-10.0,
                                 10.0,
                                 10.0,
                                 -10.0);

            m_Sut.Line = other;

            Assert.AreEqual(other,
                            m_Sut.Line);
        }

        [Test]
        public void LineDirection_ReturnsDefault()
        {
            Assert.AreEqual(m_LineDirection,
                            m_Sut.LineDirection);
        }

        [Test]
        public void LineDirection_Roundtrip()
        {
            const Constants.LineDirection other = Constants.LineDirection.Reverse;

            m_Sut.LineDirection = other;

            Assert.AreEqual(other,
                            m_Sut.LineDirection);
        }

        [Test]
        public void Points_ReturnsDefault()
        {
            System.Windows.Point[] actual =
                new LineToWindowPointsConverter(m_GeometryPointToWindowsPointConverter).Points.ToArray();

            Assert.AreEqual(0,
                            actual.Length);
        }

        [Test]
        public void Points_ReturnsFirstPoint()
        {
            System.Windows.Point[] actual = m_Sut.Points.ToArray();

            System.Windows.Point point = actual [ 0 ];

            Assert.AreEqual(m_WindowsStartPoint,
                            point);
        }

        [Test]
        public void Points_ReturnsSecondPoint()
        {
            System.Windows.Point[] actual = m_Sut.Points.ToArray();

            System.Windows.Point point = actual [ 1 ];

            Assert.AreEqual(m_WindowsEndPoint,
                            point);
        }

        [Test]
        public void Points_ReturnsTwo_ForLength()
        {
            System.Windows.Point[] actual = m_Sut.Points.ToArray();

            Assert.AreEqual(2,
                            actual.Length);
        }
    }
}