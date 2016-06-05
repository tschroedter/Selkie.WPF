using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry.Primitives;
using Selkie.Geometry.Shapes;
using Selkie.NUnit.Extensions;
using Selkie.WPF.Common.Interfaces.Converters;
using Constants = Selkie.Geometry.Constants;
using Point = System.Windows.Point;

namespace Selkie.WPF.Common.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class DisplayLineTests
    {
        [SetUp]
        public void Setup()
        {
            m_PointOne = new Point(-10.0,
                                   -10.0);
            m_PointTwo = new Point(10.0,
                                   10.0);
            m_Points = new[]
                       {
                           m_PointOne,
                           m_PointTwo
                       };

            m_Line = new Line(0,
                              -10.0,
                              0.0,
                              10.0,
                              20.0);

            m_Converter = Substitute.For <ILineToWindowPointsConverter>();
            m_Converter.Points.Returns(m_Points);

            m_Sut = new DisplayLine(m_Converter,
                                    m_Line);
        }

        private Point m_PointOne;
        private Point m_PointTwo;
        private DisplayLine m_Sut;
        private ILineToWindowPointsConverter m_Converter;
        private Point[] m_Points;
        private ILine m_Line;

        private Point CreatePointForDegrees(double degrees)
        {
            var circle = new Circle(0.0,
                                    0.0,
                                    10.0);
            Angle angle = Angle.FromDegrees(degrees);
            Geometry.Shapes.Point point = circle.PointOnCircle(angle);

            return new Point(point.X,
                             point.Y);
        }

        [Test]
        public void CalculateAngle_Returns0Degrees_ForPointAt0Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            var endPoint = new Point(10.0,
                                     0.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(0.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns0Degrees_ForPointAt0Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            var endPoint = new Point(10.0,
                                     0.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(180.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns135Degrees_ForPointAt135Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(135.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(135.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns135Degrees_ForPointAt135Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(135.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(315.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns180Degrees_ForPointAt180Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(180.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(180.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns180Degrees_ForPointAt180Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(180.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(0.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns225Degrees_ForPointAt225Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(225.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(225.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns225Degrees_ForPointAt225Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(225.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(45.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns270Degrees_ForPointAt270Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(270.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(270.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns270Degrees_ForPointAt270Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(270.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(90.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns315Degrees_ForPointAt315Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(315.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(315.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns45Degrees_ForPointAt45Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(45.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(45.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns45Degrees_ForPointAt45Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(45.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(225.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns90Degrees_ForPointAt90Degrees_Forward()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(90.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertDegrees(90.0,
                                      actual);
        }

        [Test]
        public void CalculateAngle_Returns90Degrees_ForPointAt90Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(90.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(270.0,
                                      actual);
        }

        [Test]
        public void CalculateAngleFor_Returns315Degrees_ForPointAt315Degrees_Reverse()
        {
            // Arrange
            var startPoint = new Point(0.0,
                                       0.0);
            Point endPoint = CreatePointForDegrees(315.0);

            // Act
            double actual = m_Sut.CalculateAngle(startPoint,
                                                 endPoint,
                                                 Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertDegrees(135.0,
                                      actual);
        }

        [Test]
        public void DirectionAngle_Returns45Degrees()
        {
            NUnitHelper.AssertDegrees(45.0,
                                      m_Sut.DirectionAngle);
        }

        [Test]
        public void EndPoint_ReturnsPoint()
        {
            Assert.AreEqual(m_PointTwo,
                            m_Sut.EndPoint);
        }

        [Test]
        public void Id_ReturnsLineId()
        {
            Assert.AreEqual(m_Line.Id,
                            m_Sut.Id);
        }

        [Test]
        public void Name_ReturnsString()
        {
            const string expected = "Line 0 [-10,-10 - 10,10] @45.00deg";
            string actual = m_Sut.Name;

            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void StartPoint_ReturnsPoint()
        {
            Assert.AreEqual(m_PointOne,
                            m_Sut.StartPoint);
        }

        [Test]
        public void X1_ReturnsX1()
        {
            Assert.AreEqual(m_PointOne.X,
                            m_Sut.X1);
        }

        [Test]
        public void X2_ReturnsX2()
        {
            Assert.AreEqual(m_PointTwo.X,
                            m_Sut.X2);
        }

        [Test]
        public void Y1_ReturnsY1()
        {
            Assert.AreEqual(m_PointOne.Y,
                            m_Sut.Y1);
        }

        [Test]
        public void Y2_ReturnsY2()
        {
            Assert.AreEqual(m_PointTwo.Y,
                            m_Sut.Y2);
        }
    }
}