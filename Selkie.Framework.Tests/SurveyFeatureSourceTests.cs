using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class SurveyFeatureSourceTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = Substitute.For <ISurveyFeaturesToCostPerSurveyFeatureConverter>();
            m_Converter.CostPerFeature.Returns(new[]
                                               {
                                                   2
                                               });
            m_Lines = new ILine[]
                      {
                          new Line(0,
                                   1.0,
                                   1.0,
                                   3.0,
                                   1.0),
                          new Line(1,
                                   11.0,
                                   11.0,
                                   30.0,
                                   1.0)
                      };

            m_Sut = new SurveyFeatureSource(m_Converter,
                                            m_Lines);
        }

        private SurveyFeatureSource m_Sut;
        private ISurveyFeaturesToCostPerSurveyFeatureConverter m_Converter;
        private ILine[] m_Lines;

        [Test]
        public void Constructor_CallsConvert_WhenCalled()
        {
            m_Converter.Received().Convert();
        }

        [Test]
        public void Constructor_ConvertsAllLinesToSurveyPolylines_WhenCalled()
        {
            Assert.AreEqual(m_Lines.Length,
                            m_Sut.SurveyPolylines.Count());
        }

        [Test]
        public void Constructor_SetsCostPerFeature_WhenCalled()
        {
            Assert.AreEqual(m_Converter.CostPerFeature,
                            m_Sut.CostPerFeature);
        }

        [Test]
        public void Constructor_SetsFeatures_WhenCalled()
        {
            Assert.AreEqual(m_Lines.Length,
                            m_Converter.Features.Count());
        }

        [Test]
        public void Constructor_SetsLines_WhenCalled()
        {
            Assert.True(m_Sut.Lines.Any());
        }
    }
}