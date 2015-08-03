using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Model = new NodeModel(1,
                                    2.0,
                                    3.0,
                                    Angle.For45Degrees);
        }

        private NodeModel m_Model;

        [Test]
        public void DirectionAngle_ReturnsValue_WhenCalled()
        {
            Assert.AreEqual(Angle.For45Degrees,
                            m_Model.DirectionAngle);
        }

        [Test]
        public void Id_ReturnsValue_WhenCalled()
        {
            Assert.AreEqual(1,
                            m_Model.Id);
        }

        [Test]
        public void IsUnknown_ReturnsFalse_ForKnownNode()
        {
            Assert.False(m_Model.IsUnknown);
        }

        [Test]
        public void IsUnknown_ReturnsTrue_ForUnknownNode()
        {
            Assert.True(NodeModel.Unknown.IsUnknown);
        }

        [Test]
        public void Unknown_SetsProperties_WhenCreated()
        {
            INodeModel actual = NodeModel.Unknown;

            Assert.AreEqual(NodeModel.UnknownId,
                            actual.Id,
                            "Id");
            Assert.AreEqual(0.0,
                            actual.X,
                            "X");
            Assert.AreEqual(0.0,
                            actual.Y,
                            "Y");
            Assert.AreEqual(Angle.ForZeroDegrees,
                            actual.DirectionAngle,
                            "DirectionAngle");
        }

        [Test]
        public void X_ReturnsValue_WhenCalled()
        {
            Assert.AreEqual(2.0,
                            m_Model.X);
        }

        [Test]
        public void Y_ReturnsValue_WhenCalled()
        {
            Assert.AreEqual(3.0,
                            m_Model.Y);
        }
    }
}