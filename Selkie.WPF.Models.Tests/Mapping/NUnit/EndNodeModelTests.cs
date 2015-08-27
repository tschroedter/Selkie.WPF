using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class EndNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_MemoryBus = Substitute.For <ISelkieInMemoryBus>();
            m_Helper = Substitute.For <INodeIdHelper>();

            m_Model = new EndNodeModel(m_Bus,
                                       m_MemoryBus,
                                       m_Helper);
        }

        private ISelkieBus m_Bus;
        private EndNodeModel m_Model;
        private INodeIdHelper m_Helper;
        private ISelkieInMemoryBus m_MemoryBus;

        [Test]
        public void DetermieNodeIdReversesTest()
        {
            m_Helper.Reverse(3).Returns(2);

            int actual = m_Model.DetermineNodeId(new[]
                                                 {
                                                     1,
                                                     3
                                                 });

            Assert.AreEqual(2,
                            actual);
        }

        [Test]
        public void SendsMessageTest()
        {
            m_Model.SendMessage();

            m_MemoryBus.Received()
                       .Publish(Arg.Any <EndNodeModelChangedMessage>());
        }
    }
}