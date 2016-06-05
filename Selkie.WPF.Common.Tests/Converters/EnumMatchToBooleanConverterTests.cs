using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using NUnit.Framework;
using Selkie.WPF.Common.Converters;

namespace Selkie.WPF.Common.Tests.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class EnumMatchToBooleanConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_DoesNotMatter = typeof( object );
            m_Culture = CultureInfo.InstalledUICulture;

            m_Converter = new EnumMatchToBooleanConverter();
        }

        private enum EnumTypes
        {
            One
        }

        private EnumMatchToBooleanConverter m_Converter;
        private Type m_DoesNotMatter;
        private CultureInfo m_Culture;

        [Test]
        public void Convert_ReturnsFalse_ForParameterIsNull()
        {
            // Arrange
            // Act
            var actual = ( bool ) m_Converter.Convert("Name",
                                                      m_DoesNotMatter,
                                                      null,
                                                      m_Culture);

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void Convert_ReturnsFalse_ForValueAndParameterDoNotMatch()
        {
            // Arrange
            // Act
            var actual = ( bool ) m_Converter.Convert("Name",
                                                      m_DoesNotMatter,
                                                      "Other",
                                                      m_Culture);

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void Convert_ReturnsFalse_ForValueIsNull()
        {
            // Arrange
            // Act
            var actual = ( bool ) m_Converter.Convert(null,
                                                      m_DoesNotMatter,
                                                      "Name",
                                                      m_Culture);

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void Convert_ReturnsTrue_ForValueAndParameterMatch()
        {
            // Arrange
            // Act
            var actual = ( bool ) m_Converter.Convert("Name",
                                                      m_DoesNotMatter,
                                                      "Name",
                                                      m_Culture);

            // Assert
            Assert.True(actual);
        }

        [Test]
        public void ConvertBack_ReturnsDoNothing_ForValueIsFalse()
        {
            // Arrange
            // Act
            object actual = m_Converter.ConvertBack(false,
                                                    typeof( EnumTypes ),
                                                    "One",
                                                    m_Culture);

            // Assert
            Assert.AreEqual(Binding.DoNothing,
                            actual);
        }

        [Test]
        public void ConvertBack_ReturnsEnum_ForValueIsTrue()
        {
            // Arrange
            // Act
            object actual = m_Converter.ConvertBack(true,
                                                    typeof( EnumTypes ),
                                                    "One",
                                                    m_Culture);

            // Assert
            Assert.AreEqual(EnumTypes.One,
                            actual);
        }

        [Test]
        public void ConvertBack_ReturnsFalse_ForParameterIsNull()
        {
            // Arrange
            // Act
            object actual = m_Converter.ConvertBack("One",
                                                    m_DoesNotMatter,
                                                    null,
                                                    m_Culture);

            // Assert
            Assert.AreEqual(Binding.DoNothing,
                            actual);
        }

        [Test]
        public void ConvertBack_ReturnsFalse_ForValueIsNull()
        {
            // Arrange
            // Act
            object actual = m_Converter.ConvertBack(null,
                                                    m_DoesNotMatter,
                                                    "One",
                                                    m_Culture);

            // Assert
            Assert.AreEqual(Binding.DoNothing,
                            actual);
        }
    }
}