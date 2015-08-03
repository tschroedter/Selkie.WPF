using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.Models.TrailHistory;

namespace Selkie.WPF.Models.Tests.TrailHistory.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class TrailHistoryModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Factory = Substitute.For <ITrailDetailsFactory>();

            m_Model = new TrailHistoryModel(m_Logger,
                                            m_Bus,
                                            m_Factory);
        }

        [TearDown]
        public void Teardown()
        {
            m_Model.Dispose();
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private ITrailDetailsFactory m_Factory;
        private TrailHistoryModel m_Model;

        private ColonyBestTrailMessage CreateBestTrailMessage()
        {
            var message = new ColonyBestTrailMessage
                          {
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3,
                              Iteration = 1,
                              Length = 1000.0,
                              Trail = new[]
                                      {
                                          2,
                                          0
                                      },
                              Type = "Type"
                          };

            return message;
        }

        private ColonyBestTrailMessage CreateOtherBestTrailMessage()
        {
            var message = new ColonyBestTrailMessage
                          {
                              Alpha = 0.1,
                              Beta = 0.2,
                              Gamma = 0.3,
                              Iteration = 2,
                              Length = 750.0,
                              Trail = new[]
                                      {
                                          2,
                                          0
                                      },
                              Type = "Type"
                          };

            return message;
        }

        private ITrailDetails Create(CallInfo callInfo)
        {
            return new TrailDetails(( int ) callInfo [ 0 ],
                                    ( int[] ) callInfo [ 1 ],
                                    ( double ) callInfo [ 2 ],
                                    ( double ) callInfo [ 3 ],
                                    ( double ) callInfo [ 4 ],
                                    ( string ) callInfo [ 5 ],
                                    ( double ) callInfo [ 6 ],
                                    ( double ) callInfo [ 7 ],
                                    ( double ) callInfo [ 8 ]);
        }

        private static ITrailDetails[] CreateList()
        {
            var one = Substitute.For <ITrailDetails>();
            one.Interation.Returns(1);
            var two = Substitute.For <ITrailDetails>();
            two.Interation.Returns(2);
            var three = Substitute.For <ITrailDetails>();
            three.Interation.Returns(3);

            var list = new[]
                       {
                           one,
                           two,
                           three
                       };
            return list;
        }

        [Test]
        public void BestTrailHandlerCallsUpdateTest()
        {
            m_Model.ColonyBestTrailHandler(CreateBestTrailMessage());

            m_Bus.Received().Publish(Arg.Any <TrailHistoryModelChangedMessage>());
        }

        [Test]
        public void CreateOtherTrailDetailsTest()
        {
            var first = new TrailDetails(1,
                                         new int[]
                                         {
                                         },
                                         2000.0,
                                         3000.0,
                                         4000.0,
                                         "Type",
                                         5.0,
                                         6.0,
                                         7.0);

            var other = new TrailDetails(10,
                                         new int[]
                                         {
                                         },
                                         200.0,
                                         300.0,
                                         400.0,
                                         "Other Type",
                                         5.0,
                                         6.0,
                                         7.0);

            m_Model.CreateOtherTrailDetails(first,
                                            other);

            m_Factory.Received().Create(other.Interation,
                                        other.Trail,
                                        other.Length,
                                        1800.0,
                                        90.0,
                                        other.Type,
                                        other.Alpha,
                                        other.Beta,
                                        other.Gamma);
        }

        [Test]
        public void CreateTrailDetailsListAddsFirstToListTest()
        {
            ITrailDetails[] list = CreateList();

            ITrailDetails expected = list.First();
            ITrailDetails actual = m_Model.CreateTrailDetailsList(list).First();

            Assert.AreEqual(expected,
                            actual);
        }

        [Test]
        public void CreateTrailDetailsListCallsCreateOtherTrailDetailsTest()
        {
            ITrailDetails[] list = CreateList();

            m_Model.CreateTrailDetailsList(list);

            m_Factory.ReceivedWithAnyArgs(2).Create(0,
                                                    new int[]
                                                    {
                                                    },
                                                    0.0,
                                                    0.0,
                                                    0.0,
                                                    "Type",
                                                    1.0,
                                                    2.0,
                                                    3.0);
        }

        [Test]
        public void CreateTrailDetailsListCountTest()
        {
            ITrailDetails[] list = CreateList();

            List <ITrailDetails> actual = m_Model.CreateTrailDetailsList(list);

            Assert.AreEqual(3,
                            actual.Count());
        }

        [Test]
        public void CreateTrailDetailsTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            m_Model.CreateTrailDetails(message);

            m_Factory.Received().Create(1,
                                        message.Trail,
                                        message.Length,
                                        0.0,
                                        0.0,
                                        message.Type,
                                        message.Alpha,
                                        message.Beta,
                                        message.Gamma);
        }

        [Test]
        public void DefaultTrailsTest()
        {
            Assert.NotNull(m_Model.Trails);
        }

        [Test]
        public void DisposCallsReleaseTrailDetailsTest()
        {
            var factory = Substitute.For <ITrailDetailsFactory>();
            var model = new TrailHistoryModel(m_Logger,
                                              m_Bus,
                                              factory);

            model.Update(CreateBestTrailMessage());

            model.Dispose();

            factory.Received().Release(Arg.Any <ITrailDetails>());
        }

        [Test]
        public void StartedCallsReleaseTest()
        {
            m_Model.Update(CreateBestTrailMessage());

            Assert.AreEqual(1,
                            m_Model.Trails.Count(),
                            "Precondition!");

            m_Model.Started();

            m_Factory.Received().Release(Arg.Any <ITrailDetails>());
        }

        [Test]
        public void StartedClearsTrailsTest()
        {
            m_Model.Update(CreateBestTrailMessage());

            Assert.AreEqual(1,
                            m_Model.Trails.Count(),
                            "Precondition!");

            m_Model.Started();

            Assert.AreEqual(0,
                            m_Model.Trails.Count());
        }

        [Test]
        public void StartedHandlerCallsStartedTest()
        {
            m_Model.Update(CreateBestTrailMessage());

            Assert.AreEqual(1,
                            m_Model.Trails.Count(),
                            "Precondition!");

            m_Model.ColonyStartRequestHandler(new ColonyStartRequestMessage());

            Assert.AreEqual(0,
                            m_Model.Trails.Count());
        }

        [Test]
        public void StartedSetsFirstTrailDetailsToUnknownTest()
        {
            m_Model.Update(CreateBestTrailMessage());

            Assert.False(m_Model.FirsTrailDetails.IsUnknown);

            m_Model.Started();

            Assert.True(m_Model.FirsTrailDetails.IsUnknown);
        }

        [Test]
        public void UpdateAddsTest()
        {
            m_Model.Update(CreateBestTrailMessage());

            IEnumerable <ITrailDetails> actual = m_Model.Trails;

            Assert.AreEqual(1,
                            actual.Count());
        }

        [Test]
        public void UpdateCallsFactoryForFirstTrailDetailsTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            m_Model.Update(message);

            m_Factory.Received().Create(1,
                                        message.Trail,
                                        message.Length,
                                        0.0,
                                        0.0,
                                        message.Type,
                                        message.Alpha,
                                        message.Beta,
                                        message.Gamma);
        }

        [Test]
        public void UpdateCallsFactoryForOtherTrailDetailsTest()
        {
            m_Factory.Create(0,
                             new int[]
                             {
                             },
                             0.0,
                             0.0,
                             0.0,
                             "type",
                             0.0,
                             0.0,
                             0.0).ReturnsForAnyArgs(Create);

            m_Model.Update(CreateBestTrailMessage());

            ColonyBestTrailMessage messageOther = CreateOtherBestTrailMessage();
            m_Model.Update(messageOther);

            m_Factory.Received().Create(2,
                                        messageOther.Trail,
                                        messageOther.Length,
                                        250.0,
                                        25.0,
                                        messageOther.Type,
                                        messageOther.Alpha,
                                        messageOther.Beta,
                                        messageOther.Gamma);
        }

        [Test]
        public void UpdateCreatesSortedListTest()
        {
            m_Factory.Create(0,
                             new int[]
                             {
                             },
                             0.0,
                             0.0,
                             0.0,
                             "type",
                             0.0,
                             0.0,
                             0.0).ReturnsForAnyArgs(Create);

            m_Model.Update(CreateOtherBestTrailMessage());
            m_Model.Update(CreateBestTrailMessage());

            ITrailDetails[] actual = m_Model.Trails.ToArray();

            Assert.AreEqual(2,
                            actual.Length,
                            "Length");
            Assert.AreEqual(1,
                            actual [ 0 ].Interation,
                            "One");
            Assert.AreEqual(2,
                            actual [ 1 ].Interation,
                            "Two");
        }

        [Test]
        public void UpdateRemovesAndAddsTest()
        {
            m_Model.Update(CreateBestTrailMessage());
            m_Model.Update(CreateBestTrailMessage());

            IEnumerable <ITrailDetails> actual = m_Model.Trails;

            Assert.AreEqual(1,
                            actual.Count());
        }

        [Test]
        public void UpdateSendsMessageTest()
        {
            ColonyBestTrailMessage message = CreateBestTrailMessage();

            m_Model.Update(message);

            m_Bus.Received().Publish(Arg.Any <TrailHistoryModelChangedMessage>());
        }
    }
}