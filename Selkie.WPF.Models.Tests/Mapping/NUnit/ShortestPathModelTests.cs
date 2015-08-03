using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class ShortestPathModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_NodeToDisplayLineConverter = Substitute.For <ILineToLineNodeConverterToDisplayLineConverter>();

            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Converter = Substitute.For <IPathToLineToLineNodeConverter>();
            m_Factory = Substitute.For <ILineToLineNodeConverterToDisplayLineConverterFactory>();
            m_Factory.Create().Returns(m_NodeToDisplayLineConverter);

            m_Model = new ShortestPathModel(m_Logger,
                                            m_Bus,
                                            m_Converter,
                                            m_Factory);
        }

        private ShortestPathModel m_Model;
        private ILogger m_Logger;
        private IBus m_Bus;
        private IPathToLineToLineNodeConverter m_Converter;
        private ILineToLineNodeConverterToDisplayLineConverterFactory m_Factory;
        private ILineToLineNodeConverterToDisplayLineConverter m_NodeToDisplayLineConverter;

        private ColonyBestTrailMessage CreateBestTrailMessage()
        {
            var message = new ColonyBestTrailMessage
                          {
                              Iteration = 1,
                              Trail = new[]
                                      {
                                          0,
                                          1
                                      },
                              Length = 123.0,
                              Type = "Type",
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3
                          };

            return message;
        }

        [Test]
        public void BestTrailHandlerCallsUpdateTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };
            m_Converter.Nodes.Returns(nodes);

            m_Model.ColonyBestTrailHandler(message);

            Assert.True(nodes.SequenceEqual(m_Model.Nodes));
        }

        [Test]
        public void ConvertPathConvertsPathToNodesTest()
        {
            m_Converter.ClearReceivedCalls();

            ColonyBestTrailMessage message = CreateBestTrailMessage();

            m_Model.ConvertPath(message.Trail);

            Assert.AreEqual(message.Trail,
                            m_Converter.Path,
                            "Path");
            m_Converter.Received().Convert();
        }

        [Test]
        public void DisposeReleasesConverterTest()
        {
            m_Model.Dispose();

            m_Factory.Received().Release(m_NodeToDisplayLineConverter);
        }

        [Test]
        public void SubscribesToBestTrailMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyBestTrailMessage, Task>>());
        }

        [Test]
        public void UpdateConverterCreatesNewConverterTest()
        {
            var displayLines = new IDisplayLine[0];
            var converter = Substitute.For <ILineToLineNodeConverterToDisplayLineConverter>();
            converter.DisplayLines.Returns(displayLines);

            m_Factory.Create().Returns(converter);

            m_Model.UpdateConverter();

            Assert.AreEqual(displayLines,
                            m_Model.Path);
        }

        [Test]
        public void UpdateConverterReleasesOldConverterTest()
        {
            m_Model.UpdateConverter();

            m_Factory.Received().Release(m_NodeToDisplayLineConverter);
        }

        [Test]
        public void UpdateConvertsPathToNodesTest()
        {
            m_Converter.ClearReceivedCalls();

            ColonyBestTrailMessage message = CreateBestTrailMessage();

            m_Model.Update(message);

            Assert.AreEqual(message.Trail,
                            m_Converter.Path,
                            "Path");
            m_Converter.Received().Convert();
        }

        [Test]
        public void UpdateNodesTest()
        {
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };
            m_Converter.Nodes.Returns(nodes);

            m_Model.UpdateNodes();

            Assert.True(nodes.SequenceEqual(m_Model.Nodes));
        }

        [Test]
        public void UpdateSendsMessageTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();
            m_Bus.ClearReceivedCalls();

            m_Model.Update(message);

            m_Bus.Received().Publish(Arg.Any <ShortestPathModelChangedMessage>());
        }

        [Test]
        public void UpdateUpdatesNodesTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();
            var nodes = new[]
                        {
                            Substitute.For <ILineToLineNodeConverter>()
                        };
            m_Converter.Nodes.Returns(nodes);

            m_Model.Update(message);

            Assert.True(nodes.SequenceEqual(m_Model.Nodes));
        }
    }
}