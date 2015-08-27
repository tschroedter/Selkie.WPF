using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class NodesModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Lines = CreateLines();

            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieBus>();
            m_MemoryBus = Substitute.For <ISelkieInMemoryBus>();
            m_Manager = Substitute.For <ILinesSourceManager>();

            m_Model = new NodesModel(m_Logger,
                                     m_Bus,
                                     m_MemoryBus,
                                     m_Manager);
        }

        private ISelkieLogger m_Logger;
        private ISelkieBus m_Bus;
        private ILinesSourceManager m_Manager;
        private NodesModel m_Model;
        private IEnumerable <ILine> m_Lines;
        private ISelkieInMemoryBus m_MemoryBus;

        private IEnumerable <Line> CreateLines()
        {
            var line1StartPoint = new Point(30.0,
                                            0.0);
            var line1EndPoint = new Point(40.0,
                                          0.0);
            var line1 = new Line(0,
                                 line1StartPoint,
                                 line1EndPoint);

            var line2StartPoint = new Point(0.0,
                                            40.0);
            var line2EndPoint = new Point(60.0,
                                          40.0);
            var line2 = new Line(1,
                                 line2StartPoint,
                                 line2EndPoint);

            var line3StartPoint = new Point(-30.0,
                                            80.0);
            var line3EndPoint = new Point(90.0,
                                          80.0);
            var line3 = new Line(2,
                                 line3StartPoint,
                                 line3EndPoint);

            var line4StartPoint = new Point(-30.0,
                                            -80.0);
            var line4EndPoint = new Point(90.0,
                                          -80.0);
            var line4 = new Line(3,
                                 line4StartPoint,
                                 line4EndPoint);

            return new List <Line>
                   {
                       line1,
                       line2,
                       line3,
                       line4
                   };
        }

        [Test]
        public void CreateModelsCreatesTwoModelsTest()
        {
            var line = new Line(1,
                                2.0,
                                3.0,
                                4.0,
                                5.0);

            IEnumerable <INodeModel> actual = m_Model.CreateNodeModels(line);

            Assert.AreEqual(2.0,
                            actual.Count());
        }

        [Test]
        public void CreateModelsFinishModelTest()
        {
            var line = new Line(1,
                                2.0,
                                3.0,
                                4.0,
                                5.0);

            INodeModel actual = m_Model.CreateNodeModels(line).Last();

            Assert.AreEqual(3,
                            actual.Id,
                            "Id");
            Assert.AreEqual(4.0,
                            actual.X,
                            "X");
            Assert.AreEqual(5.0,
                            actual.Y,
                            "Y");
        }

        [Test]
        public void CreateModelsStartModelTest()
        {
            var line = new Line(1,
                                2.0,
                                3.0,
                                4.0,
                                5.0);

            INodeModel actual = m_Model.CreateNodeModels(line).First();

            Assert.AreEqual(2,
                            actual.Id,
                            "Id");
            Assert.AreEqual(2.0,
                            actual.X,
                            "X");
            Assert.AreEqual(3.0,
                            actual.Y,
                            "Y");
        }

        [Test]
        public void DefaultNodesTest()
        {
            Assert.NotNull(m_Model.Nodes);
        }

        [Test]
        public void LinesChangedHandlerCallsLoadNodesTest()
        {
            var message = new ColonyLinesChangedMessage();

            m_Model.LinesChangedHandler(message);

            m_MemoryBus.Received()
                       .Publish(Arg.Any <NodesModelChangedMessage>());
        }

        [Test]
        public void LoadNodesCountTest()
        {
            m_Manager.Lines.Returns(m_Lines);

            m_Model.LoadNodes();

            int count = m_Lines.Count() * 2; // each line has a start and finish node

            Assert.AreEqual(count,
                            m_Model.Nodes.Count());
        }
    }
}