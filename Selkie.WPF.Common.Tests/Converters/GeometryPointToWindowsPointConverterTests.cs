using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Converters;

namespace Selkie.WPF.Common.Tests.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class GeometryPointToWindowsPointConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Point1 = new Point(1.0,
                                 2.0);
            m_Point2 = new Point(3.0,
                                 4.0);

            m_Sut = new GeometryPointToWindowsPointConverter();
        }

        private GeometryPointToWindowsPointConverter m_Sut;
        private Point m_Point1;
        private Point m_Point2;

        [Test]
        public void Convert_CalculatesPoint_ForWindowsPoint()
        {
            // Arrange
            var expected = new System.Windows.Point(GeometryPointToWindowsPointConverter.Origin.X + 1.0,
                                                    GeometryPointToWindowsPointConverter.Origin.Y + -2.0);

            // Act
            m_Sut.GeometryPoint = m_Point1;
            m_Sut.Convert();

            // Assert
            Assert.AreEqual(expected.X,
                            m_Sut.Point.X,
                            "X");
            Assert.AreEqual(expected.Y,
                            m_Sut.Point.Y,
                            "Y");
        }

        [Test]
        public void Point_Roundtrip()
        {
            // Arrange
            m_Sut.GeometryPoint = m_Point1;
            Assert.AreEqual(m_Point1,
                            m_Sut.GeometryPoint,
                            "Point1");

            // Act
            m_Sut.GeometryPoint = m_Point2;

            // Assert
            Assert.AreEqual(m_Point2,
                            m_Sut.GeometryPoint,
                            "Point2");
        }
    }
}