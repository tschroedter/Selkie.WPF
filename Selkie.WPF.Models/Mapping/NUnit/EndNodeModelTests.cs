using System.Diagnostics.CodeAnalysis;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;

namespace Selkie.WPF.Models.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class EndNodeModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Helper = Substitute.For <INodeIdHelper>();

            m_Model = new EndNodeModel(m_Logger,
                                       m_Bus,
                                       m_Helper);
        }

        private IBus m_Bus;
        private ILogger m_Logger;
        private EndNodeModel m_Model;
        private INodeIdHelper m_Helper;

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

            m_Bus.Received().Publish(Arg.Any <EndNodeModelChangedMessage>());
        }
    }
}