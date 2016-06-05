using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.WPF.Models.Settings;

namespace Selkie.WPF.Models.Tests.Settings
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class AntSettingsNodeTests
    {
        [Test]
        public void Constructor_SetsDescription_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsNode(1,
                                          "Text");

            // Assert
            Assert.AreEqual("Text",
                            sut.Description);
        }

        [Test]
        public void Constructor_SetsId_WhenCalled()
        {
            // Arrange
            // Act
            var sut = new AntSettingsNode(1,
                                          "Text");

            // Assert
            Assert.AreEqual(1,
                            sut.Id);
        }
    }
}