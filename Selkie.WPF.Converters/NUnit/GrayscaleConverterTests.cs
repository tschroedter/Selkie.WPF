using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Selkie.NUnit.Extensions;

namespace Selkie.WPF.Converters.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class GrayscaleConverterTests
    {
        [SetUp]
        public void Setup()
        {
            var converter = new DoubleToIntegerConverter();

            m_Converter = new GrayscaleConverter(converter);
        }

        private GrayscaleConverter m_Converter;

        private readonly double[][] m_Pheromones =
        {
            new[]
            {
                -64.0,
                -64.0,
                -64.0
            },
            new[]
            {
                0.0,
                0.0,
                0.0
            },
            new[]
            {
                64.0,
                64.0,
                64.0
            }
        };

        [Test]
        public void ConverterTest()
        {
            m_Converter.Minimum = -64.0;
            m_Converter.Maximum = 64.0;
            m_Converter.NumberOfPossibleValues = 10;
            m_Converter.Pheromones = m_Pheromones;

            m_Converter.Convert();

            List <int>[] actual = m_Converter.Grayscale.ToArray();

            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                0,
                                                0,
                                                0
                                            },
                                            actual [ 0 ],
                                            "[0]");
            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                5,
                                                5,
                                                5
                                            },
                                            actual [ 1 ],
                                            "[1]");
            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                9,
                                                9,
                                                9
                                            },
                                            actual [ 2 ],
                                            "[2]");
        }

        [Test]
        public void MaximumDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(128.0,
                                           m_Converter.Maximum);
        }

        [Test]
        public void MaximumRoundtripTest()
        {
            m_Converter.Maximum = 255.0;

            NUnitHelper.AssertIsEquivalent(255.0,
                                           m_Converter.Maximum);
        }

        [Test]
        public void MinimumDefaultTest()
        {
            NUnitHelper.AssertIsEquivalent(-128.0,
                                           m_Converter.Minimum);
        }

        [Test]
        public void MinimumRoundtripTest()
        {
            m_Converter.Minimum = -255.0;

            NUnitHelper.AssertIsEquivalent(-255.0,
                                           m_Converter.Minimum);
        }

        [Test]
        public void NumberOfPossibleValuesDefaultTest()
        {
            Assert.AreEqual(255,
                            m_Converter.NumberOfPossibleValues);
        }

        [Test]
        public void NumberOfPossibleValuesRoundtripTest()
        {
            m_Converter.NumberOfPossibleValues = 128;

            Assert.AreEqual(128,
                            m_Converter.NumberOfPossibleValues);
        }

        [Test]
        public void PheromonesDefaultTest()
        {
            Assert.NotNull(m_Converter.Pheromones,
                           "Pheromones");
            Assert.NotNull(m_Converter.Pheromones [ 0 ],
                           "Pheromones[0]");
        }

        [Test]
        public void PheromonesRoundtripTest()
        {
            m_Converter.Pheromones = m_Pheromones;

            Assert.AreEqual(m_Pheromones,
                            m_Converter.Pheromones);
        }

        [Test]
        public void ToIntegerTest()
        {
            m_Converter.Minimum = -64.0;
            m_Converter.Maximum = 64.0;
            m_Converter.NumberOfPossibleValues = 10;

            int[][] actual = m_Converter.ToInteger(m_Pheromones,
                                                   10).ToArray();

            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                0,
                                                0,
                                                0
                                            },
                                            actual [ 0 ],
                                            "[0]");
            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                5,
                                                5,
                                                5
                                            },
                                            actual [ 1 ],
                                            "[1]");
            NUnitHelper.AssertSequenceEqual(new[]
                                            {
                                                9,
                                                9,
                                                9
                                            },
                                            actual [ 2 ],
                                            "[2]");
        }
    }
}