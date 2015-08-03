using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.TrailHistory;

namespace Selkie.WPF.Models.Tests.TrailHistory.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class TrailDetailsTests
    {
        [SetUp]
        public void Setup()
        {
            m_Trail = new[]
                      {
                          0,
                          2
                      };

            m_TrailDetails = new TrailDetails(1,
                                              m_Trail,
                                              1.0,
                                              2.0,
                                              3.0,
                                              "Type",
                                              4.0,
                                              5.0,
                                              6.0);
        }

        private TrailDetails m_TrailDetails;
        private int[] m_Trail;

        [Test]
        public void AlphaTest()
        {
            Assert.AreEqual(4.0,
                            m_TrailDetails.Alpha);
        }

        [Test]
        public void BetaTest()
        {
            Assert.AreEqual(5.0,
                            m_TrailDetails.Beta);
        }

        [Test]
        public void GammaTest()
        {
            Assert.AreEqual(6.0,
                            m_TrailDetails.Gamma);
        }

        [Test]
        public void InterationTest()
        {
            Assert.AreEqual(1,
                            m_TrailDetails.Interation);
        }

        [Test]
        public void IsUnknownReturnsFalseForKnownTest()
        {
            Assert.False(m_TrailDetails.IsUnknown);
        }

        [Test]
        public void IsUnknownReturnsTrueForUnknownTest()
        {
            ITrailDetails actual = TrailDetails.Unknown;

            Assert.True(actual.IsUnknown);
        }

        [Test]
        public void LengthDeltaInPercentTest()
        {
            Assert.AreEqual(3.0,
                            m_TrailDetails.LengthDeltaInPercent);
        }

        [Test]
        public void LengthDeltaTest()
        {
            Assert.AreEqual(2.0,
                            m_TrailDetails.LengthDelta);
        }

        [Test]
        public void LengthTest()
        {
            Assert.AreEqual(1.0,
                            m_TrailDetails.Length);
        }

        [Test]
        public void TrailTest()
        {
            Assert.AreEqual(m_Trail,
                            m_TrailDetails.Trail);
        }

        [Test]
        public void TypeTest()
        {
            Assert.AreEqual("Type",
                            m_TrailDetails.Type);
        }
    }
}