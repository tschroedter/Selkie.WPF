using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Selkie.Geometry.Shapes;
using Selkie.NUnit.Extensions;
using Constants = Selkie.Common.Constants;

namespace Selkie.WPF.Converters.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LineToLineNodeConverterTests
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

            m_Converter = new LineToLineNodeConverter
                          {
                              From = m_From,
                              FromDirection = m_FromDirection,
                              To = m_To,
                              ToDirection = m_ToDirection
                          };

            m_Converter.Convert();
        }

        private LineToLineNodeConverter m_Converter;
        private Line m_From;
        private Constants.LineDirection m_FromDirection;
        private Line m_To;
        private Constants.LineDirection m_ToDirection;
        private Point m_FromStartPoint;
        private Point m_FromEndPoint;
        private Point m_ToStartPoint;
        private Point m_ToEndPoint;

        [Test]
        public void CalculateCostStartToStartTest()
        {
            double actual = m_Converter.CalculateCostStartToStart(m_From,
                                                                  m_FromDirection,
                                                                  m_To,
                                                                  m_ToDirection);

            NUnitHelper.AssertIsEquivalent(169.70,
                                           actual,
                                           NUnit.Extensions.Constants.EpsilonDistance,
                                           "CostStartToStart");
        }

        [Test]
        public void CalculateCostTest()
        {
            double actual = m_Converter.CalculateCost(m_From,
                                                      m_FromDirection,
                                                      m_To,
                                                      m_ToDirection);

            NUnitHelper.AssertIsEquivalent(240.42,
                                           actual,
                                           NUnit.Extensions.Constants.EpsilonDistance,
                                           "CostStartToStart");
        }

        [Test]
        public void CostTest()
        {
            NUnitHelper.AssertIsEquivalent(240.42,
                                           m_Converter.Cost,
                                           NUnit.Extensions.Constants.EpsilonDistance,
                                           "Cost");
        }

        [Test]
        public void CreatePointsForTransferForForwardForwardTest()
        {
            Point[] actual = m_Converter.Points.ToArray();

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
        public void EqualsReturnsFalseForOtherIsNullTest()
        {
            Assert.False(m_Converter.Equals(null));
        }

        [Test]
        public void EqualsReturnsFalseForOtherTypeTest()
        {
            Assert.False(m_Converter.Equals(new object()));
        }

        [Test]
        public void EqualsReturnsTrueForOtherSameValuesTest()
        {
            var other = new LineToLineNodeConverter
                        {
                            From = m_From,
                            FromDirection = m_FromDirection,
                            To = m_To,
                            ToDirection = m_ToDirection
                        };

            other.Convert();

            Assert.True(m_Converter.Equals(other));
        }

        [Test]
        public void EqualsReturnsTrueForSameObjectTest()
        {
            var other = new LineToLineNodeConverter
                        {
                            From = m_From,
                            FromDirection = m_FromDirection,
                            To = m_To,
                            ToDirection = m_ToDirection
                        };

            other.Convert();

            Assert.True(m_Converter.Equals(( object ) other));
        }

        [Test]
        public void EqualsReturnsTrueForSameTest()
        {
            Assert.True(m_Converter.Equals(m_Converter));
        }

        [Test]
        public void FromDefaultTest()
        {
            var converter = new LineToLineNodeConverter();

            Assert.AreEqual(Line.Unknown,
                            converter.From);
        }

        [Test]
        public void FromDirectionDefaultTest()
        {
            var converter = new LineToLineNodeConverter();

            Assert.AreEqual(Constants.LineDirection.Forward,
                            converter.FromDirection);
        }

        [Test]
        public void FromDirectionRoundtripTest()
        {
            m_Converter.FromDirection = Constants.LineDirection.Reverse;

            Assert.AreEqual(Constants.LineDirection.Reverse,
                            m_Converter.FromDirection);
        }

        [Test]
        public void FromDirectionTest()
        {
            Assert.AreEqual(m_FromDirection,
                            m_Converter.FromDirection);
        }

        [Test]
        public void FromRoundtripTest()
        {
            m_Converter.From = m_To;

            Assert.AreEqual(m_To,
                            m_Converter.From);
        }

        [Test]
        public void FromTest()
        {
            Assert.AreEqual(m_From,
                            m_Converter.From);
        }

        [Test]
        public void GetHashCodeTest()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.DoesNotThrow(() => m_Converter.GetHashCode());
        }

        [Test]
        public void PointsCountTest()
        {
            Assert.AreEqual(2,
                            m_Converter.Points.Count());
        }

        [Test]
        public void ToDefaultTest()
        {
            var converter = new LineToLineNodeConverter();

            Assert.AreEqual(Line.Unknown,
                            converter.To);
        }

        [Test]
        public void ToDirectionDefaultTest()
        {
            var converter = new LineToLineNodeConverter();

            Assert.AreEqual(Constants.LineDirection.Forward,
                            converter.ToDirection);
        }

        [Test]
        public void ToDirectionRoundtripTest()
        {
            m_Converter.ToDirection = Constants.LineDirection.Reverse;

            Assert.AreEqual(Constants.LineDirection.Reverse,
                            m_Converter.ToDirection);
        }

        [Test]
        public void ToDirectionTest()
        {
            Assert.AreEqual(m_ToDirection,
                            m_Converter.ToDirection);
        }

        [Test]
        public void ToRoundtripTest()
        {
            m_Converter.To = m_From;

            Assert.AreEqual(m_From,
                            m_Converter.To);
        }

        [Test]
        public void ToStringTest()
        {
            const string expected = "0 (Forward) - 1 (Forward)";
            string actual = m_Converter.ToString();

            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void ToTest()
        {
            Assert.AreEqual(m_To,
                            m_Converter.To);
        }
    }
}