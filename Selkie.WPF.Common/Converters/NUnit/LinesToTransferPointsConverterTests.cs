using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Selkie.Common;
using Selkie.Geometry.Shapes;

namespace Selkie.WPF.Common.Converters.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class LinesToTransferPointsConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_FromStartPoint = new Point(10.0,
                                         10.0);
            m_FromEndPoint = new Point(60.0,
                                       60.0);
            m_ToStartPoint = new Point(-10.0,
                                       -10.0);
            m_ToEndPoint = new Point(-60.0,
                                     -60.0);

            m_From = new Line(0,
                              m_FromStartPoint,
                              m_FromEndPoint);
            m_FromDirection = Constants.LineDirection.Forward;
            m_To = new Line(1,
                            m_ToStartPoint,
                            m_ToEndPoint);
            m_ToDirection = Constants.LineDirection.Forward;

            m_Sut = new LinesToTransferPointsConverter(m_From,
                                                       m_FromDirection,
                                                       m_To,
                                                       m_ToDirection);

            m_Sut.Convert();
        }

        private LinesToTransferPointsConverter m_Sut;
        private Line m_From;
        private Constants.LineDirection m_FromDirection;
        private Point m_FromEndPoint;
        private Point m_FromStartPoint;
        private Line m_To;
        private Constants.LineDirection m_ToDirection;
        private Point m_ToEndPoint;
        private Point m_ToStartPoint;

        [Test]
        public void CreatePointsForTransfer_ReturnsPoints_ForForwardForward()
        {
            // Arrange
            // Act
            Point[] actual = m_Sut.CreatePointsForTransfer(m_From,
                                                           Constants.LineDirection.Forward,
                                                           m_To,
                                                           Constants.LineDirection.Forward).ToArray();

            // Assert
            Assert.AreEqual(2,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_FromEndPoint,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_ToStartPoint,
                            actual [ 1 ],
                            "[1]");
        }

        [Test]
        public void CreatePointsForTransfer_ReturnsPoints_ForForwardReverse()
        {
            // Arrange
            // Act
            Point[] actual = m_Sut.CreatePointsForTransfer(m_From,
                                                           Constants.LineDirection.Forward,
                                                           m_To,
                                                           Constants.LineDirection.Reverse).ToArray();

            // Assert
            Assert.AreEqual(2,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_FromEndPoint,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_ToEndPoint,
                            actual [ 1 ],
                            "[1]");
        }

        [Test]
        public void CreatePointsForTransfer_ReturnsPoints_ForReverseForward()
        {
            // Arrange
            // Act
            Point[] actual = m_Sut.CreatePointsForTransfer(m_From,
                                                           Constants.LineDirection.Reverse,
                                                           m_To,
                                                           Constants.LineDirection.Forward).ToArray();

            // Assert
            Assert.AreEqual(2,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_FromStartPoint,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_ToStartPoint,
                            actual [ 1 ],
                            "[1]");
        }

        [Test]
        public void CreatePointsForTransfer_ReturnsPoints_ForReverseReverse()
        {
            // Arrange
            // Act
            Point[] actual = m_Sut.CreatePointsForTransfer(m_From,
                                                           Constants.LineDirection.Reverse,
                                                           m_To,
                                                           Constants.LineDirection.Reverse).ToArray();


            // Assert
            Assert.AreEqual(2,
                            actual.Length,
                            "Length");
            Assert.AreEqual(m_FromStartPoint,
                            actual [ 0 ],
                            "[0]");
            Assert.AreEqual(m_ToEndPoint,
                            actual [ 1 ],
                            "[1]");
        }

        [Test]
        public void From_ReturnsFrom_AfterCallingConvert()
        {
            Assert.AreEqual(m_From,
                            m_Sut.From);
        }

        [Test]
        public void FromDirection_ReturnsDirection_AfterCallingConvert()
        {
            Assert.AreEqual(m_FromDirection,
                            m_Sut.FromDirection);
        }

        [Test]
        public void Points_ReturnsTwoPoints_AfterCallingConvert()
        {
            Assert.AreEqual(2,
                            m_Sut.Points.Count());
        }

        [Test]
        public void To_ReturnsTo_AfterCallingConvert()
        {
            Assert.AreEqual(m_To,
                            m_Sut.To);
        }

        [Test]
        public void ToDirection_ReturnsToDirection_AfterCallingConvert()
        {
            Assert.AreEqual(m_ToDirection,
                            m_Sut.ToDirection);
        }
    }
}