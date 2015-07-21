using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Geometry.Shapes;
using Selkie.NUnit.Extensions;
using Constants = Selkie.Common.Constants;

namespace Selkie.WPF.Common.Converters.NUnit
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class CostForLineSwitchCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
            m_OtherDirection = Constants.LineDirection.Reverse;
            m_OtherLine = new Line(2,
                                   100.0,
                                   100.0,
                                   600.0,
                                   600.0);

            m_From = new Line(0,
                              10.0,
                              10.0,
                              60.0,
                              60.0);
            m_FromDirection = Constants.LineDirection.Forward;
            m_To = new Line(1,
                            -10.0,
                            -10.0,
                            -60.0,
                            -60.0);
            m_ToDirection = Constants.LineDirection.Forward;

            m_Sut = new CostForLineSwitchCalculator(m_From,
                                                    m_FromDirection,
                                                    m_To,
                                                    m_ToDirection);

            m_Sut.Calculate();
        }

        private Line m_From;
        private Constants.LineDirection m_FromDirection;
        private Line m_To;
        private Constants.LineDirection m_ToDirection;
        private CostForLineSwitchCalculator m_Sut;
        private Line m_OtherLine;
        private Constants.LineDirection m_OtherDirection;

        [Test]
        public void CostDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(98.99,
                                           m_Sut.Cost,
                                           Geometry.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void CostForLineSwitch_ReturnsValue_ForForwardForward()
        {
            // Arrange
            // Act
            double actual = m_Sut.CostForLineSwitch(m_From,
                                                    m_FromDirection,
                                                    m_To,
                                                    m_ToDirection);

            // Assert
            NUnitHelper.AssertIsEquivalent(98.99,
                                           actual,
                                           Geometry.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void CostForLineSwitch_ReturnsValue_ForForwardReverse()
        {
            // Arrange
            // Act
            double actual = m_Sut.CostForLineSwitch(m_From,
                                                    Constants.LineDirection.Forward,
                                                    m_To,
                                                    Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertIsEquivalent(169.70,
                                           actual,
                                           Geometry.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void CostForLineSwitch_ReturnsValue_ForReverseForward()
        {
            // Arrange
            // Act
            double actual = m_Sut.CostForLineSwitch(m_From,
                                                    Constants.LineDirection.Reverse,
                                                    m_To,
                                                    Constants.LineDirection.Forward);

            // Assert
            NUnitHelper.AssertIsEquivalent(28.28,
                                           actual,
                                           Geometry.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void CostForLineSwitch_ReturnsValue_ForReverseReverse()
        {
            // Arrange
            // Act
            double actual = m_Sut.CostForLineSwitch(m_From,
                                                    Constants.LineDirection.Reverse,
                                                    m_To,
                                                    Constants.LineDirection.Reverse);

            // Assert
            NUnitHelper.AssertIsEquivalent(98.99,
                                           actual,
                                           Geometry.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void From_ReturnsDefault()
        {
            Assert.AreEqual(m_From,
                            m_Sut.From);
        }

        [Test]
        public void From_Roundtrip()
        {
            // Arrange
            // Act
            m_Sut.From = m_OtherLine;

            // Assert
            Assert.AreEqual(m_OtherLine,
                            m_Sut.From);
        }

        [Test]
        public void FromDirection_ReturnsDefault()
        {
            Assert.AreEqual(m_FromDirection,
                            m_Sut.FromDirection);
        }

        [Test]
        public void FromDirection_Roundtrip()
        {
            // Arrange
            // Act
            m_Sut.FromDirection = m_OtherDirection;

            // Assert
            Assert.AreEqual(m_OtherDirection,
                            m_Sut.FromDirection);
        }

        [Test]
        public void To_ReturnsDefault()
        {
            Assert.AreEqual(m_To,
                            m_Sut.To);
        }

        [Test]
        public void To_Roundtrip()
        {
            // Arrange
            // Act
            m_Sut.To = m_OtherLine;

            // Assert
            Assert.AreEqual(m_OtherLine,
                            m_Sut.To);
        }

        [Test]
        public void ToDirection_ReturnsDefault()
        {
            Assert.AreEqual(m_ToDirection,
                            m_Sut.ToDirection);
        }

        [Test]
        public void ToDirection_Roundtrip()
        {
            // Arrange
            // Act
            m_Sut.ToDirection = m_OtherDirection;

            // Assert
            Assert.AreEqual(m_OtherDirection,
                            m_Sut.ToDirection);
        }
    }
}