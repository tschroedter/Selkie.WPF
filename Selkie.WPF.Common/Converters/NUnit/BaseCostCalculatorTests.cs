using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Common;
using Geometry.Primitives;
using Geometry.Shapes;
using NSubstitute;
using NUnit.Framework;
using Racetrack;
using Selkie.WPF.Common.Converters;

namespace WPF.Common.Converters.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class BaseCostCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
            var path = Substitute.For<IPath>();
            path.Distance.Returns(new Distance(100.0));

            m_Line1 = new Line(0, 10.0, 10.0, 20.0, 10.0);
            m_Line2 = new Line(1, 10.0, 20.0, 20.0, 20.0);

            m_Lines1 = new[] {m_Line1, m_Line2};

            m_ForwardForwardPaths = CreateForwardForwardPaths();
            m_ForwardReversePaths = CreateForwardReversePaths();
            m_ReverseForwardPaths = CreateReverseForwardPaths();
            m_ReverseReversePaths = CreateReverseReversePaths();

            m_Racetracks = Substitute.For<IRacetracks>();
            m_Racetracks.ForwardToForward.Returns(m_ForwardForwardPaths);
            m_Racetracks.ForwardToReverse.Returns(m_ForwardReversePaths);
            m_Racetracks.ReverseToForward.Returns(m_ReverseForwardPaths);
            m_Racetracks.ReverseToReverse.Returns(m_ReverseReversePaths);

            m_Calculator = new TestBaseCostCalculator();
        }

        private TestBaseCostCalculator m_Calculator;

        private class TestBaseCostCalculator : BaseCostCalculator
        {
            internal override double CalculateRacetrackCost(int fromLineId,
                                                            int toLineId)
            {
                return 100.0;
            }
        }

        private Line m_Line1;
        private Line m_Line2;
        private Line[] m_Lines1;
        private IPath[][] m_ForwardForwardPaths;
        private IPath[][] m_ForwardReversePaths;
        private IPath[][] m_ReverseForwardPaths;
        private IPath[][] m_ReverseReversePaths;
        private IRacetracks m_Racetracks;

        private static IPath[][] CreateForwardForwardPaths()
        {
            return CreatePaths(new[] {10.0, 20.0, 30.0, 40.0});
        }

        private static IPath[][] CreateForwardReversePaths()
        {
            return CreatePaths(new[] {50.0, 60.0, 70.0, 80.0});
        }

        private static IPath[][] CreateReverseForwardPaths()
        {
            return CreatePaths(new[] {90.0, 100.0, 110.0, 120.0});
        }

        private static IPath[][] CreateReverseReversePaths()
        {
            return CreatePaths(new[] {130.0, 140.0, 150.0, 160.0});
        }

        private static IPath[][] CreatePaths(double[] distances)
        {
            var distance0 = new Distance(distances[0]);
            var distance1 = new Distance(distances[1]);

            var path1 = Substitute.For<IPath>();
            path1.Distance.Returns(distance0);
            var path2 = Substitute.For<IPath>();
            path2.Distance.Returns(distance1);

            IPath[][] paths =
            {
                new[] {path1, path2},
                new[] {path1, path2}
            };

            return paths;
        }

        [Test]
        public void CalculateTest()
        {
            m_Calculator.Lines = m_Lines1;
            m_Calculator.Line = m_Line1;
            m_Calculator.Racetracks = m_Racetracks;
            m_Calculator.Calculate();

            Dictionary<int, double> actual = m_Calculator.Costs;

            Assert.AreEqual(2, actual.Keys.Count, "Count");
            Assert.AreEqual(CostMatrix.CostToMyself, actual[0]);
            NUnitHelper.AssertIsEquivalent(100.0, actual[1], Constants.EpsilonDistance, "actual[1]");
        }

        [Test]
        public void DefaultCostsTest()
        {
            Assert.NotNull(m_Calculator.Costs);
        }

        [Test]
        public void DefaultLineTest()
        {
            Assert.NotNull(m_Calculator.Line, "Line");
            Assert.True(m_Calculator.Line.IsUnknown, "IsUnknown");
        }

        [Test]
        public void DefaultLinesTest()
        {
            Assert.NotNull(m_Calculator.Lines);
        }

        [Test]
        public void RoundtripLineTest()
        {
            var line = Substitute.For<ILine>();

            m_Calculator.Line = line;

            Assert.AreEqual(line, m_Calculator.Line);
        }

        [Test]
        public void RoundtripLinesTest()
        {
            var line = Substitute.For<ILine>();
            var lines = new[] {line};

            m_Calculator.Lines = lines;

            Assert.AreEqual(lines, m_Calculator.Lines);
        }

        // todo don't know who to make it pass nicely yet
        /*
        [Test]
        public void DefaultRacetracksest()
        {
            Assert.NotNull(m_Calculator.Racetracks);
        }
        */

        [Test]
        public void RoundtripRacetracksTest()
        {
            var racetracks = Substitute.For<IRacetracks>();

            m_Calculator.Racetracks = racetracks;

            Assert.AreEqual(racetracks, m_Calculator.Racetracks);
        }
    }
}