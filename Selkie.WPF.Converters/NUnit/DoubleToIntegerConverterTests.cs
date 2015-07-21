using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.NUnit.Extensions;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class DoubleToIntegerConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = new DoubleToIntegerConverter();
        }

        private DoubleToIntegerConverter m_Converter;

        [Test]
        public void DoubleToIntegerForAverageValueTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     -5.0,
                                                     1.0,
                                                     0.0);

            Assert.AreEqual(4,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForIntervalIsNegativeTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     0.0,
                                                     -1.0,
                                                     10.0);

            Assert.AreEqual(0,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForIntervalIsZeroTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     0.0,
                                                     0.0,
                                                     10.0);

            Assert.AreEqual(0,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForLessThanMinimumValueTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     -5.0,
                                                     1.0,
                                                     -1000.0);

            Assert.AreEqual(0,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForMaximumValueTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     -5.0,
                                                     1.0,
                                                     5.0);

            Assert.AreEqual(9,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForMinimumValueTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     -5.0,
                                                     1.0,
                                                     -5.0);

            Assert.AreEqual(0,
                            actual);
        }

        [Test]
        public void DoubleToIntegerForMoreThanMaximumValueTest()
        {
            int actual = m_Converter.DoubleToInteger(10,
                                                     -5.0,
                                                     1.0,
                                                     1000.0);

            Assert.AreEqual(9,
                            actual);
        }

        [Test]
        public void IntegerDefaultTest()
        {
            Assert.AreEqual(0,
                            m_Converter.Integer);
        }

        [Test]
        public void IntegerTest()
        {
            m_Converter.NumberOfPossibleValues = 10;
            m_Converter.Minimum = -64.0;
            m_Converter.Interval = 10;
            m_Converter.Value = 0.0;

            m_Converter.Convert();

            Assert.AreEqual(6,
                            m_Converter.Integer);
        }

        [Test]
        public void IntervalDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(0.0,
                                           m_Converter.Interval);
        }

        [Test]
        public void IntervalRoundtripTest()
        {
            m_Converter.Interval = 255.0;

            NUnitHelper.AssertIsEquivalent(255.0,
                                           m_Converter.Interval);
        }

        [Test]
        public void MinimumDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(0.0,
                                           m_Converter.Minimum);
        }

        [Test]
        public void MinimumRoundtripTest()
        {
            m_Converter.Minimum = 255.0;

            NUnitHelper.AssertIsEquivalent(255.0,
                                           m_Converter.Minimum);
        }

        [Test]
        public void NumberOfPossibleValuesDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(0,
                                           m_Converter.NumberOfPossibleValues);
        }

        [Test]
        public void NumberOfPossibleValuesRoundtripTest()
        {
            m_Converter.NumberOfPossibleValues = 255;

            Assert.AreEqual(255,
                            m_Converter.NumberOfPossibleValues);
        }

        [Test]
        public void ValueDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(0.0,
                                           m_Converter.Value);
        }

        [Test]
        public void ValueRoundtripTest()
        {
            m_Converter.Value = 255.0;

            NUnitHelper.AssertIsEquivalent(255.0,
                                           m_Converter.Value);
        }
    }
}