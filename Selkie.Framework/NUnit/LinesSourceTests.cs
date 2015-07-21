using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;

namespace Selkie.Framework.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LinesSourceTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = Substitute.For <ILinesToCostPerLineConverter>();
            m_Converter.CostPerLine.Returns(new[]
                                            {
                                                2
                                            });
            m_Lines = new ILine[]
                      {
                          new Line(0,
                                   1.0,
                                   1.0,
                                   3.0,
                                   1.0)
                      };

            m_Sut = new LinesSource(m_Converter,
                                    m_Lines);
        }

        private LinesSource m_Sut;
        private ILinesToCostPerLineConverter m_Converter;
        private ILine[] m_Lines;

        [Test]
        public void Constructor_CallsConvert_WhenCreated()
        {
            m_Converter.Received().Convert();
        }

        [Test]
        public void Constructor_SetsCostPerLine_WhenCreated()
        {
            Assert.AreEqual(m_Converter.CostPerLine,
                            m_Sut.CostPerLine);
        }

        [Test]
        public void Constructor_SetsLines_WhenCreated()
        {
            Assert.AreEqual(m_Lines,
                            m_Converter.Lines);
        }

        [Test]
        public void LinesTest()
        {
            Assert.IsNotNull(m_Sut.Lines);
            Assert.True(m_Sut.Lines.Any());
        }
    }
}