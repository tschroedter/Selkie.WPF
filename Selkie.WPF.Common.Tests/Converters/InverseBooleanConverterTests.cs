using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NUnit.Framework;
using Selkie.WPF.Common.Converters;

namespace Selkie.WPF.Common.Tests.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class InverseBooleanConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_TargetType = typeof( bool );
            m_Culture = CultureInfo.InstalledUICulture;

            m_Sut = new InverseBooleanConverter();
        }

        private InverseBooleanConverter m_Sut;
        private CultureInfo m_Culture;
        private Type m_TargetType;

        [Test]
        public void Convert_ReturnsFalse_ForTrue()
        {
            // Arrange
            // Act
            object condition = m_Sut.Convert(true,
                                             m_TargetType,
                                             null,
                                             m_Culture);

            // Assert
            Assert.False(( bool ) condition);
        }

        [Test]
        public void Convert_ReturnsTrue_ForFalse()
        {
            // Arrange
            // Act
            object condition = m_Sut.Convert(false,
                                             m_TargetType,
                                             null,
                                             m_Culture);

            // Assert
            Assert.True(( bool ) condition);
        }

        [Test]
        public void ConvertBack_ReturnsFalse_ForTrue()
        {
            // Arrange
            // Act
            object condition = m_Sut.ConvertBack(true,
                                                 m_TargetType,
                                                 null,
                                                 m_Culture);

            // Assert
            Assert.False(( bool ) condition);
        }

        [Test]
        public void ConvertBack_ReturnsTrue_ForFalse()
        {
            // Arrange
            // Act
            object condition = m_Sut.ConvertBack(false,
                                                 m_TargetType,
                                                 null,
                                                 m_Culture);

            // Assert
            Assert.True(( bool ) condition);
        }
    }
}