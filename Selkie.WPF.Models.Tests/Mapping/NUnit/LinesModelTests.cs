using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EasyNetQ;
using NSubstitute;
using NUnit.Framework;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Mapping;

namespace Selkie.WPF.Models.Tests.Mapping.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class LinesModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Lines = CreateLines();

            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Manager.Lines.Returns(m_Lines);

            m_Logger = Substitute.For <ILogger>();
            m_Bus = Substitute.For <IBus>();
            m_Manager = Substitute.For <ILinesSourceManager>();
            m_Factory = Substitute.For <IDisplayLineFactory>();

            m_Model = new LinesModel(m_Logger,
                                     m_Bus,
                                     m_Manager,
                                     m_Factory);
        }

        [TearDown]
        public void TearDown()
        {
            m_Model.Dispose();
        }

        private ILogger m_Logger;
        private IBus m_Bus;
        private ILinesSourceManager m_Manager;
        private IDisplayLineFactory m_Factory;
        private LinesModel m_Model;
        private IEnumerable <ILine> m_Lines;

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
        public void ConstructorLoadsLinesTest()
        {
            m_Manager.Lines.Returns(m_Lines);

            var model = new LinesModel(m_Logger,
                                       m_Bus,
                                       m_Manager,
                                       m_Factory);

            Assert.AreEqual(m_Lines.Count(),
                            model.Lines.Count());

            model.Dispose();
        }

        [Test]
        public void DisposeCallsReleaseDisplayLinesTest()
        {
            m_Manager.Lines.Returns(m_Lines);

            var model = new LinesModel(m_Logger,
                                       m_Bus,
                                       m_Manager,
                                       m_Factory);

            model.Dispose();

            int count = m_Lines.Count();

            m_Factory.Received(count).Release(Arg.Any <IDisplayLine>());
        }

        [Test]
        public void LinesChangedHandlerCallsUpdateTest()
        {
            var message = new ColonyLinesChangedMessage();

            m_Model.ColonyLinesChangedHandler(message);

            m_Bus.Received().PublishAsync(Arg.Any <LinesModelChangedMessage>());
        }

        [Test]
        public void LoadDisplayLinesAddsLinesInCorrectOrderTest()
        {
            m_Model.ClearDisplayLines();

            m_Model.LoadDisplayLines(m_Lines);

            for ( var i = 0 ; i < m_Lines.Count() ; i++ )
            {
                ILine expected = m_Lines.ElementAt(0);
                IDisplayLine actual = m_Model.Lines.ElementAt(0);

                Assert.AreEqual(expected.Id,
                                actual.Id,
                                "Wrong line at index " + i + "!");
            }
        }

        [Test]
        public void LoadDisplayLinesNumberOfLinesTest()
        {
            m_Model.ClearDisplayLines();

            m_Model.LoadDisplayLines(m_Lines);

            Assert.AreEqual(m_Lines.Count(),
                            m_Model.Lines.Count());
        }

        [Test]
        public void LoadDisplaySendsMessageTest()
        {
            m_Bus.ClearReceivedCalls();

            m_Model.LoadDisplayLines(m_Lines);

            m_Bus.Received().PublishAsync(Arg.Any <LinesModelChangedMessage>());
        }

        [Test]
        public void ReleaseDisplayLinesTest()
        {
            m_Model.LoadDisplayLines(m_Lines);

            m_Model.ReleaseDisplayLines();

            int count = m_Lines.Count();

            m_Factory.Received(count).Release(Arg.Any <IDisplayLine>());
        }

        [Test]
        public void SubscribeToLinesChangedMessageTest()
        {
            m_Bus.Received().SubscribeAsync(m_Model.GetType().FullName,
                                            Arg.Any <Func <ColonyLinesChangedMessage, Task>>());
        }

        [Test]
        public void UpdateCallsLoadDisplayLinesTest()
        {
            m_Model.Update(new ILine[0]);

            m_Bus.Received().PublishAsync(Arg.Any <LinesModelChangedMessage>());
        }

        [Test]
        public void UpdateCallsReleaseDisplayLinesTest()
        {
            // Arrange
            m_Model.Update(m_Lines);

            // Act
            m_Model.Update(new ILine[0]);

            // Assert
            m_Factory.Received(m_Lines.Count())
                     .Release(Arg.Any <IDisplayLine>());
        }
    }
}