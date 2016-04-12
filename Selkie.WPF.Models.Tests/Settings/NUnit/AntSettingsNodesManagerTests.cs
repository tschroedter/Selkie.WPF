using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.Settings;

namespace Selkie.WPF.Models.Tests.Settings.NUnit
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class AntSettingsNodesManagerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Factory = Substitute.For <IAntSettingsNodeFactory>();
            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Converter = Substitute.For <ILinesToNodesConverter>();

            m_Sut = new AntSettingsNodesManager(m_Factory,
                                                m_Manager,
                                                m_Converter);
        }

        private IAntSettingsNodeFactory m_Factory;
        private ILinesSourceManager m_Manager;
        private ILinesToNodesConverter m_Converter;
        private AntSettingsNodesManager m_Sut;

        private IEnumerable <IAntSettingsNode> CreatedTestNodes()
        {
            var one = Substitute.For <IAntSettingsNode>();
            one.Id.Returns(0);

            var two = Substitute.For <IAntSettingsNode>();
            two.Id.Returns(1);

            var nodes = new[]
                        {
                            one,
                            two
                        };

            return nodes;
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

        [Test]
        public void CreateNodesForCurrentLines_CallsConverter_WhenCalled()
        {
            // Arrange
            ILine[] lines = CreateTestLines().ToArray();
            m_Manager.Lines.Returns(lines);

            // Act
            m_Sut.CreateNodesForCurrentLines();

            // Assert
            m_Converter.Received().Convert(m_Factory,
                                           lines);
        }

        [Test]
        public void CreateNodesForCurrentLines_CallsReleasesOldNodes_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> nodes = CreatedTestNodes();
            m_Converter.Nodes.Returns(nodes);

            ILine[] lines = CreateTestLines().ToArray();
            m_Manager.Lines.Returns(lines);

            m_Sut.CreateNodesForCurrentLines();

            // Act
            m_Sut.CreateNodesForCurrentLines();

            // Assert
            m_Factory.Received(2).Release(Arg.Any <IAntSettingsNode>());
        }

        [Test]
        public void CreateNodesForCurrentLines_SetsNodes_WhenCalled()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> nodes = CreatedTestNodes();
            m_Converter.Nodes.Returns(nodes);

            ILine[] lines = CreateTestLines().ToArray();
            m_Manager.Lines.Returns(lines);

            // Act
            m_Sut.CreateNodesForCurrentLines();

            // Assert
            Assert.AreEqual(nodes,
                            m_Sut.Nodes);
        }

        [Test]
        public void Nodes_ReturnsDefault_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0,
                            m_Sut.Nodes.Count());
        }

        [Test]
        public void ReleaseNodes_CallsFactory_ForGivenNodes()
        {
            // Arrange
            IEnumerable <IAntSettingsNode> nodes = CreatedTestNodes();

            // Act
            m_Sut.ReleaseNodes(nodes);

            // Assert
            m_Factory.Received(2).Release(Arg.Any <IAntSettingsNode>());
        }
    }
}