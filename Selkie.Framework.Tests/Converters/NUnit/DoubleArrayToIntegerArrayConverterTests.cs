using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Framework.Converters;

namespace Selkie.Framework.Tests.Converters.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class DoubleArrayToIntegerArrayConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_DoubleMatrix = new[]
                             {
                                 new[]
                                 {
                                     1.5,
                                     2.5
                                 },
                                 new[]
                                 {
                                     3.5,
                                     4.5
                                 }
                             };
        }

        private double[][] m_DoubleMatrix;

        [Test]
        public void Convert_SetsIntegerMatrix_WhenCalled()
        {
            // Arrange
            var sut = new DoubleArrayToIntegerArrayConverter
                      {
                          DoubleMatrix = m_DoubleMatrix
                      };

            // Act
            sut.Convert();

            // Assert
            Assert.AreEqual(1,
                            sut.IntegerMatrix [ 0 ] [ 0 ],
                            "[0,0]");
            Assert.AreEqual(2,
                            sut.IntegerMatrix [ 0 ] [ 1 ],
                            "[0,1]");
            Assert.AreEqual(3,
                            sut.IntegerMatrix [ 1 ] [ 0 ],
                            "[1,0]");
            Assert.AreEqual(4,
                            sut.IntegerMatrix [ 1 ] [ 1 ],
                            "[1,1]");
        }

        [Test]
        public void DoubleMatrix_Roundtrip()
        {
            // Arrange
            // Act
            var sut = new DoubleArrayToIntegerArrayConverter
                      {
                          DoubleMatrix = m_DoubleMatrix
                      };

            // Assert
            Assert.AreEqual(m_DoubleMatrix,
                            sut.DoubleMatrix);
        }
    }
}