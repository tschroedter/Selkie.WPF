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
    internal sealed class StartNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_MemoryBus = Substitute.For <ISelkieInMemoryBus>();
            m_Helper = Substitute.For <INodeIdHelper>();

            m_Model = new StartNodeModel(m_Bus,
                                         m_MemoryBus,
                                         m_Helper);
        }

        private ISelkieBus m_Bus;
        private StartNodeModel m_Model;
        private INodeIdHelper m_Helper;
        private ISelkieInMemoryBus m_MemoryBus;

        [Test]
        public void DetermieNodeIdTest()
        {
            int actual = m_Model.DetermineNodeId(new[]
                                                 {
                                                     1,
                                                     2,
                                                     3
                                                 });

            Assert.AreEqual(1,
                            actual);
        }

        [Test]
        public void SendsMessageTest()
        {
            m_Model.SendMessage();

            m_MemoryBus.Received()
                       .Publish(Arg.Any <StartNodeModelChangedMessage>());
        }
    }
}