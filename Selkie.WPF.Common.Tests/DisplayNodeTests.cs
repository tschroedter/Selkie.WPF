using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Interfaces.Converters;

namespace Selkie.WPF.Common.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class DisplayNodeTests
    {
        [SetUp]
        public void Setup()
        {
            m_ConverterPoint = new Point(11.0,
                                         22.0);
            m_Converter = Substitute.For <IGeometryPointToWindowsPointConverter>();
            m_Converter.Point.Returns(m_ConverterPoint);

            m_Point = new Point(1.0,
                                2.0);

            m_StrokeBrush = Brushes.Aqua;
            m_FillBrush = Brushes.Yellow;

            m_Sut = new DisplayNode(m_Converter,
                                    1,
                                    m_Point.X,
                                    m_Point.Y,
                                    3.0,
                                    4.0,
                                    m_FillBrush,
                                    m_StrokeBrush,
                                    5.0);
        }

        private IGeometryPointToWindowsPointConverter m_Converter;
        private DisplayNode m_Sut;
        private Point m_Point;
        private Point m_ConverterPoint;
        private SolidColorBrush m_StrokeBrush;
        private SolidColorBrush m_FillBrush;

        [Test]
        public void CentrePoint_ReturnsPoint()
        {
            Assert.AreEqual(m_ConverterPoint,
                            m_Sut.CentrePoint);
        }

        [Test]
        public void DirectionAngle_ReturnsAngle()
        {
            Assert.AreEqual(3.0,
                            m_Sut.DirectionAngle,
                            "DirectionAngle");
        }

        [Test]
        public void Fill_ReturnsFillBrush()
        {
            Assert.AreEqual(m_FillBrush,
                            m_Sut.Fill);
        }

        [Test]
        public void Height_ReturnsHeight()
        {
            Assert.AreEqual(8.0,
                            m_Sut.Height);
        }

        [Test]
        public void Id_ReturnsId()
        {
            Assert.AreEqual(1,
                            m_Sut.Id);
        }

        [Test]
        public void IsVisible_ReturnsFalse_ForUnknownTest()
        {
            Assert.False(DisplayNode.Unknown.IsVisible);
        }

        [Test]
        public void IsVisible_ReturnsTrue_ForKnownTest()
        {
            Assert.True(m_Sut.IsVisible);
        }

        [Test]
        public void Name_ReturnsString()
        {
            Assert.AreEqual("Node 1",
                            m_Sut.Name);
        }

        [Test]
        public void OriginalCentrePoint_ReturnsPoint()
        {
            Assert.AreEqual(m_Point,
                            m_Sut.OriginalCentrePoint);
        }

        [Test]
        public void PositionX_ReturnsX()
        {
            Assert.AreEqual(7.0,
                            m_Sut.Position.X,
                            "X");
        }

        [Test]
        public void PositionY_ReturnsY()
        {
            Assert.AreEqual(18.0,
                            m_Sut.Position.Y,
                            "Y");
        }

        [Test]
        public void Radius_ReturnsRadius()
        {
            Assert.AreEqual(4.0,
                            m_Sut.Radius);
        }

        [Test]
        public void Stroke_ReturnsStrokeBrush()
        {
            Assert.AreEqual(m_StrokeBrush,
                            m_Sut.Stroke);
        }

        [Test]
        public void StrokeThickness_ReturnsStrokeThickness()
        {
            Assert.AreEqual(5.0,
                            m_Sut.StrokeThickness);
        }

        [Test]
        public void Unknown_ReturnsFalse_ForKnown()
        {
            Assert.False(m_Sut.IsUnknown);
        }

        [Test]
        public void Unknown_ReturnsTrue_ForUnknown()
        {
            Assert.True(DisplayNode.Unknown.IsUnknown);
        }

        [Test]
        public void Width_ReturnsWidth()
        {
            Assert.AreEqual(8.0,
                            m_Sut.Width);
        }

        [Test]
        public void X_ReturnsX()
        {
            Assert.AreEqual(7.0,
                            m_Sut.X,
                            "X");
        }

        [Test]
        public void Y_ReturnsY()
        {
            Assert.AreEqual(18.0,
                            m_Sut.Y,
                            "Y");
        }
    }
}