using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using NUnit.Framework;

namespace Selkie.WPF.Converters.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class BitmapSourceConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = new BitmapSourceConverter();
        }

        private BitmapSourceConverter m_Converter;

        private readonly List <List <int>> m_Data = new List <List <int>>
                                                    {
                                                        new List <int>
                                                        {
                                                            0,
                                                            0,
                                                            0
                                                        },
                                                        new List <int>
                                                        {
                                                            128,
                                                            128,
                                                            128
                                                        },
                                                        new List <int>
                                                        {
                                                            255,
                                                            255,
                                                            255
                                                        }
                                                    };

        [Test]
        public void ConvertTest()
        {
            m_Converter.Data = m_Data;
            m_Converter.Convert();

            ImageSource actual = m_Converter.ImageSource;

            Assert.AreEqual(3,
                            actual.Width,
                            "Width");
            Assert.AreEqual(3,
                            actual.Height,
                            "Height");
        }

        [Test]
        public void DataDefaultTest()
        {
            Assert.NotNull(m_Converter.Data);
            Assert.AreEqual(0,
                            m_Converter.Data.Count,
                            "Count");
        }

        [Test]
        public void DataRoundTripTest()
        {
            var data = new List <List <int>>();

            m_Converter.Data = data;

            Assert.AreEqual(data,
                            m_Converter.Data);
        }

        [Test]
        public void DefaultDpiTest()
        {
            Assert.AreEqual(96,
                            BitmapSourceConverter.Dpi);
        }

        [Test]
        public void DefaultImageSourceTest()
        {
            Assert.NotNull(m_Converter.ImageSource);
        }

        [Test]
        public void ToImageSourceForEmptyListTest()
        {
            var data = new List <List <int>>();

            m_Converter.Data = data;

            ImageSource actual = m_Converter.ToImageSource(data);

            Assert.NotNull(actual);
        }
    }
}