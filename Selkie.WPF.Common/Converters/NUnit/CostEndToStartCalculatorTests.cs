using System.Diagnostics.CodeAnalysis;
using Common;
using Geometry.Primitives;
using NSubstitute;
using NUnit.Framework;
using Racetrack;

namespace WPF.Common.Converters.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class CostEndToStartCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
            var path = Substitute.For<IPath>();
            path.Distance.Returns(new Distance(100.0));
            m_Paths = new[] {new[] {null, path}};

            m_Racetracks = Substitute.For<IRacetracks>();

            m_Calculator = new CostEndToStartCalculator();
        }

        private CostEndToStartCalculator m_Calculator;
        private IRacetracks m_Racetracks;
        private IPath[][] m_Paths;

        [Test]
        public void CalculateRacetrackCostTest()
        {
            m_Racetracks.ForwardToForward.Returns(m_Paths);
            m_Calculator.Racetracks = m_Racetracks;

            const double expected = 100.0;
            double actual = m_Calculator.CalculateRacetrackCost(0, 1);

            NUnitHelper.AssertIsEquivalent(expected, actual, Constants.EpsilonDistance, "Racetrack length is wrong!");
        }
    }
}