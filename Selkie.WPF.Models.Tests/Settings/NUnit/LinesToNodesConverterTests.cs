using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.Settings;

namespace Selkie.WPF.Models.Tests.Settings.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LinesToNodesConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Factory = new TestAntSettingsNodeFactory();

            m_Sut = new LinesToNodesConverter();
        }

        private IAntSettingsNodeFactory m_Factory;
        private LinesToNodesConverter m_Sut;

        private static void AssertCreatedNodes(IAntSettingsNode nodeOne,
                                               IAntSettingsNode[] actual)
        {
            Assert.NotNull(nodeOne,
                           "0. node missing");
            Assert.AreEqual(0,
                            nodeOne.Id,
                            "nodeOne.Id");
            Assert.AreEqual("Line 0",
                            nodeOne.Description,
                            "nodeOne.Description");

            IAntSettingsNode nodeTwo = actual.Skip(1).First();

            Assert.NotNull(nodeTwo,
                           "1. node missing");
            Assert.AreEqual(1,
                            nodeTwo.Id,
                            "nodeTwo.Id");
            Assert.AreEqual("Line 0 (Reverse)",
                            nodeTwo.Description,
                            "nodeOne.Description");

            IAntSettingsNode nodeThree = actual.Skip(2).First();

            Assert.NotNull(nodeThree,
                           "2. node missing");
            Assert.AreEqual(2,
                            nodeThree.Id,
                            "nodeThree.Id");
            Assert.AreEqual("Line 1",
                            nodeThree.Description,
                            "nodeOne.Description");

            IAntSettingsNode nodeFour = actual.Skip(3).First();

            Assert.NotNull(nodeFour,
                           "3. node missing");
            Assert.AreEqual(3,
                            nodeFour.Id,
                            "nodeTwo.Id");
            Assert.AreEqual("Line 1 (Reverse)",
                            nodeFour.Description,
                            "nodeOne.Description");
        }

        private IEnumerable <ILine> CreateTestLines()
        {
            var one = Substitute.For <ILine>();
            one.Id.Returns(0);

            var two = Substitute.For <ILine>();
            two.Id.Returns(1);

            var lines = new[]
                        {
                            one,
                            two
                        };

            return lines;
        }

        private class TestAntSettingsNodeFactory : IAntSettingsNodeFactory
        {
            public IAntSettingsNode Create(int id,
                                           string description)
            {
                return new AntSettingsNode(id,
                                           description);
            }

            public void Release(IAntSettingsNode node)
            {
            }
        }

        [Test]
        public void Convert_CreatesCorrectNumberOfNodes_ForGivenLines()
        {
            // Arrange
            IEnumerable <ILine> lines = CreateTestLines();

            // Act
            m_Sut.Convert(m_Factory,
                          lines);

            // Assert
            Assert.AreEqual(4,
                            m_Sut.Nodes.Count());
        }

        [Test]
        public void Convert_CreatesNodes_ForGivenLines()
        {
            // Arrange
            IEnumerable <ILine> lines = CreateTestLines();

            // Act
            m_Sut.Convert(m_Factory,
                          lines);

            // Assert
            IAntSettingsNode[] actual = m_Sut.Nodes.ToArray();

            Assert.AreEqual(4,
                            actual.Length,
                            "Length");

            IAntSettingsNode nodeOne = actual.First();

            AssertCreatedNodes(nodeOne,
                               actual);
        }
    }
}