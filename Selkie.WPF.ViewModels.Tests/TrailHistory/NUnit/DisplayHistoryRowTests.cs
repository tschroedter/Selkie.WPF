using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.WPF.ViewModels.TrailHistory;

namespace Selkie.WPF.ViewModels.Tests.TrailHistory.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class DisplayHistoryRowTests
    {
        [SetUp]
        public void Setup()
        {
            m_Trail = new[]
                      {
                          1,
                          2,
                          3
                      };

            m_Row = new DisplayHistoryRow(1,
                                          m_Trail,
                                          6.0,
                                          3.0,
                                          50.0,
                                          1.0,
                                          2.0,
                                          3.0,
                                          "Type");
        }

        private DisplayHistoryRow m_Row;
        private int[] m_Trail;

        [Test]
        public void AlphaTest()
        {
            Assert.AreEqual(1.0,
                            m_Row.Alpha);
        }

        [Test]
        public void BetaTest()
        {
            Assert.AreEqual(2.0,
                            m_Row.Beta);
        }

        [Test]
        public void GammaTest()
        {
            Assert.AreEqual(3.0,
                            m_Row.Gamma);
        }

        [Test]
        public void InterationTest()
        {
            Assert.AreEqual(1,
                            m_Row.Interation);
        }

        [Test]
        public void LengthDeltaInPercentTest()
        {
            Assert.AreEqual(50.0,
                            m_Row.LengthDeltaInPercent);
        }

        [Test]
        public void LengthDeltaTest()
        {
            Assert.AreEqual(3.0,
                            m_Row.LengthDelta);
        }

        [Test]
        public void LengthTest()
        {
            Assert.AreEqual(6.0,
                            m_Row.Length);
        }

        [Test]
        public void TrailRawTest()
        {
            Assert.AreEqual("1,2,3",
                            m_Row.TrailRaw);
        }

        [Test]
        public void TrailTest()
        {
            Assert.AreEqual(m_Trail,
                            m_Row.Trail);
        }

        [Test]
        public void TypeTest()
        {
            Assert.AreEqual("Type",
                            m_Row.Type);
        }
    }
}