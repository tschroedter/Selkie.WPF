using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Messages;
using Selkie.Geometry.Shapes;
using Selkie.Services.Lines.Common;
using Selkie.Services.Lines.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class LinesSourceManager : ILinesSourceManager
    {
        private readonly ISelkieBus m_Bus;
        private readonly ITestLinesDtoToLinesConverter m_Converter;
        private readonly ILinesSourceFactory m_Factory;
        private readonly ISelkieLogger m_Logger;
        private ILinesSource m_Source = LinesSource.Unknown;

        public LinesSourceManager([NotNull] ISelkieLogger logger,
                                  [NotNull] ISelkieBus bus,
                                  [NotNull] ILinesSourceFactory factory,
                                  [NotNull] ITestLinesDtoToLinesConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_Factory = factory;
            m_Converter = converter;

            bus.PublishAsync(new TestLineRequestMessage
                             {
                                 Types = new[]
                                         {
                                             TestLineType.Type.Create45DegreeLines
                                         }
                             });

            bus.SubscribeAsync <TestLineResponseMessage>(GetType().FullName,
                                                         TestLineResponseHandler);

            bus.SubscribeAsync <ColonyLinesRequestMessage>(GetType().FullName,
                                                           ColonyLinesRequestHandler);

            bus.SubscribeAsync <ColonyTestLinesRequestMessage>(GetType().FullName,
                                                               ColonyTestLinesRequestHandler);

            bus.SubscribeAsync <ColonyTestLineSetMessage>(GetType().FullName,
                                                          ColonyTestLineSetHandler);
        }

        public IEnumerable <ILine> Lines
        {
            get
            {
                return m_Source.Lines;
            }
        }

        public IEnumerable <int> CostPerLine
        {
            get
            {
                return m_Source.CostPerLine;
            }
        }

        internal void ColonyTestLineSetHandler(ColonyTestLineSetMessage message)
        {
            TestLineType.Type type;

            if ( !Enum.TryParse(message.Type,
                                out type) )
            {
                string text = "Could not convert string {0} to TestLineType.Type!".Inject(type);
                m_Logger.Error(text);

                throw new ArgumentException(text,
                                            "message");
            }

            m_Bus.PublishAsync(new TestLineRequestMessage
                               {
                                   Types = new[]
                                           {
                                               type
                                           }
                               });
        }

        internal void ColonyLinesRequestHandler(ColonyLinesRequestMessage message)
        {
            SendColonyLinesChangedMessage();
        }

        internal void SendColonyLinesChangedMessage()
        {
            m_Bus.PublishAsync(new ColonyLinesChangedMessage());

            LogLines(m_Source.Lines);
        }

        private void LogLines([NotNull] IEnumerable <ILine> lines)
        {
            ILine[] linesArray = lines.ToArray();

            m_Logger.Info("Lines Count: {0}".Inject(linesArray.Length));

            var count = 0;
            foreach ( ILine line in linesArray )
            {
                m_Logger.Info("[{0}] {1}".Inject(count++,
                                                 line));
            }
        }

        internal void TestLineResponseHandler(TestLineResponseMessage message)
        {
            m_Factory.Release(m_Source);

            m_Converter.Dtos = message.LineDtos;
            m_Converter.Convert();

            m_Source = m_Factory.Create(m_Converter.Lines);

            SendColonyLinesChangedMessage();
            SendLinesSourceChangedMessage();
        }

        private void SendLinesSourceChangedMessage()
        {
            var response = new LinesSourceChangedMessage
                           {
                               Lines = m_Source.Lines
                           };

            m_Bus.PublishAsync(response);
        }

        internal void ColonyTestLinesRequestHandler(ColonyTestLinesRequestMessage message)
        {
            IEnumerable <string> types = GetTestLineTypes();

            var response = new ColonyTestLinesResponseMessage
                           {
                               Types = types
                           };

            m_Bus.PublishAsync(response);
        }

        public IEnumerable <string> GetTestLineTypes()
        {
            return Enum.GetNames(typeof ( TestLineType.Type )).ToList();
        }
    }
}