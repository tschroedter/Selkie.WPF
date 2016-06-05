using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class EndNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Helper = Substitute.For <INodeIdHelper>();
            m_Creator = Substitute.For <INodeModelCreator>();
            m_Creator.Helper.Returns(m_Helper);

            m_Model = new EndNodeModel(m_Bus,
                                       m_Creator);
        }

        private INodeIdHelper m_Helper;
        private EndNodeModel m_Model;
        private INodeModelCreator m_Creator;
        private ISelkieInMemoryBus m_Bus;

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

            m_Bus.Received()
                 .Publish(Arg.Any <EndNodeModelChangedMessage>());
        }
    }
}