using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Common;
using Geometry.Shapes;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LineToWindowPointsConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_StartPoint = new Point(-10.0, -10.0);
            m_EndPoint = new Point(10.0, 10.0);
            m_LineDirection = Constants.LineDirection.Forward;

            m_Line = new Line(m_StartPoint, m_EndPoint);

            m_WindowsStartPoint = new System.Windows.Point(100.0, 200.0);
            m_WindowsEndPoint = new System.Windows.Point(300.0, 400.0);

            m_GeometryPointToWindowsPointConverter = Substitute.For<IGeometryPointToWindowsPointConverter>();
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint, m_WindowsEndPoint);

            m_Converter = new LineToWindowPointsConverter(m_GeometryPointToWindowsPointConverter) {Line = m_Line};
            m_Converter.Line = m_Line;
            m_Converter.LineDirection = m_LineDirection;
            m_Converter.Convert();
        }

        private LineToWindowPointsConverter m_Converter;
        private Line m_Line;
        private Point m_StartPoint;
        private Point m_EndPoint;
        private Constants.LineDirection m_LineDirection;
        private IGeometryPointToWindowsPointConverter m_GeometryPointToWindowsPointConverter;
        private System.Windows.Point m_WindowsEndPoint;
        private System.Windows.Point m_WindowsStartPoint;

        [Test]
        public void ConvertPointsCallsConverterTest()
        {
            m_GeometryPointToWindowsPointConverter.ClearReceivedCalls();

            m_Converter.ConvertPoints(m_StartPoint, m_EndPoint);

            m_GeometryPointToWindowsPointConverter.Received(2).Convert();
        }

        [Test]
        public void ConvertPointsFirstPointTest()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint);

            System.Windows.Point[] actual = m_Converter.ConvertPoints(m_StartPoint, m_EndPoint).ToArray();

            System.Windows.Point point = actual[0];

            Assert.AreEqual(m_WindowsStartPoint, point);
        }

        [Test]
        public void ConvertPointsPointTest()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Converter.ConvertPoints(m_StartPoint, m_EndPoint).ToArray();

            System.Windows.Point point = actual[1];

            Assert.AreEqual(m_WindowsEndPoint, point);
        }

        [Test]
        public void ConvertPointsReturnsTwoPointsTest()
        {
            System.Windows.Point[] actual = m_Converter.ConvertPoints(m_StartPoint, m_EndPoint).ToArray();

            Assert.AreEqual(2, actual.Length);
        }

        [Test]
        public void CreatePointsForLineForwardTest()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint, m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Converter.CreatePointsForLine(m_Line, Constants.LineDirection.Forward).ToArray();

            Assert.AreEqual(m_WindowsStartPoint, actual[0], "Point 1");
            Assert.AreEqual(m_WindowsEndPoint, actual[1], "Point 2");
        }

        [Test]
        public void CreatePointsForLineReverseTest()
        {
            m_GeometryPointToWindowsPointConverter.Point.Returns(m_WindowsStartPoint, m_WindowsEndPoint);

            System.Windows.Point[] actual = m_Converter.CreatePointsForLine(m_Line, Constants.LineDirection.Reverse).ToArray();

            Assert.AreEqual(m_WindowsStartPoint, actual[0], "Point 1");
            Assert.AreEqual(m_WindowsEndPoint, actual[1], "Point 2");
        }

        [Test]
        public void LineDefaultTest()
        {
            Assert.AreEqual(m_Line, m_Converter.Line);
        }

        [Test]
        public void LineDirectionDefaultTest()
        {
            Assert.AreEqual(m_LineDirection, m_Converter.LineDirection);
        }

        [Test]
        public void LineDirectionRoundtripTest()
        {
            const Constants.LineDirection other = Constants.LineDirection.Reverse;

            m_Converter.LineDirection = other;

            Assert.AreEqual(other, m_Converter.LineDirection);
        }

        [Test]
        public void LineRoundtripTest()
        {
            var other = new Line(-10.0, 10.0, 10.0, -10.0);

            m_Converter.Line = other;

            Assert.AreEqual(other, m_Converter.Line);
        }

        [Test]
        public void PointsDefaultTest()
        {
            System.Windows.Point[] actual = new LineToWindowPointsConverter(m_GeometryPointToWindowsPointConverter).Points.ToArray();

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void PointsFirstPointTest()
        {
            System.Windows.Point[] actual = m_Converter.Points.ToArray();

            System.Windows.Point point = actual[0];

            Assert.AreEqual(m_WindowsStartPoint, point);
        }

        [Test]
        public void PointsLengthTest()
        {
            System.Windows.Point[] actual = m_Converter.Points.ToArray();

            Assert.AreEqual(2, actual.Length);
        }

        [Test]
        public void PointsSecondPointTest()
        {
            System.Windows.Point[] actual = m_Converter.Points.ToArray();

            System.Windows.Point point = actual[1];

            Assert.AreEqual(m_WindowsEndPoint, point);
        }

        // todo more tests
    }
}