using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Converters;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Geometry.Surveying;
using Selkie.Services.Lines.Common;
using Selkie.Services.Lines.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class LinesSourceManager : ILinesSourceManager // todo change to SurveyFeature
    {
        public LinesSourceManager([NotNull] ISelkieLogger logger,
                                  [NotNull] ISelkieBus bus,
                                  [NotNull] ISelkieInMemoryBus memoryBus,
                                  [NotNull] ILinesSourceFactory factory,
                                  [NotNull] ITestLinesDtoToLinesConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_MemoryBus = memoryBus;
            m_Factory = factory;
            m_Converter = converter;

            bus.SubscribeAsync <TestLineResponseMessage>(GetType().FullName,
                                                         TestLineResponseHandler);

            memoryBus.SubscribeAsync <ColonyLinesRequestMessage>(GetType().FullName,
                                                                 ColonyLinesRequestHandler);

            memoryBus.SubscribeAsync <ColonyAvailabeTestLinesRequestMessage>(GetType().FullName,
                                                                             ColonyAvailabeTestLinesRequestHandler);

            memoryBus.SubscribeAsync <ColonyTestLineSetMessage>(GetType().FullName,
                                                                ColonyTestLineSetHandler);
        }

        private readonly ISelkieBus m_Bus;
        private readonly ITestLinesDtoToLinesConverter m_Converter;
        private readonly ILinesSourceFactory m_Factory;
        private readonly ISelkieLogger m_Logger;
        private readonly ISelkieInMemoryBus m_MemoryBus;
        private ISurveyFeatureSource m_Source = SurveyFeatureSource.Unknown;

        public IEnumerable <ILine> Lines // todo maybe replaced with SurveyPolylines
        {
            get
            {
                return m_Source.Lines;
            }
        }

        public IEnumerable <ISurveyPolyline> SurveyPolylines // todo not used at the moment
        {
            get
            {
                return m_Source.SurveyPolylines;
            }
        }

        public IEnumerable <ISurveyFeature> SurveyFeatures
        {
            get
            {
                return m_Source.SurveyPolylines;
            }
        }

        public IEnumerable <int> CostPerFeature
        {
            get
            {
                return m_Source.CostPerFeature;
            }
        }

        public IEnumerable <string> GetTestLineTypes()
        {
            List <string> types = Enum.GetNames(typeof( TestLineType.Type )).ToList();

            types = types.Where(x => x != TestLineType.Type.CreateCrossForwardReverse.ToString()).ToList();

            return types;
        }

        internal void ColonyAvailabeTestLinesRequestHandler(ColonyAvailabeTestLinesRequestMessage message)
        {
            IEnumerable <string> types = GetTestLineTypes();

            var response = new ColonyAvailableTestLinesResponseMessage
                           {
                               Types = types
                           };

            m_MemoryBus.PublishAsync(response);
        }

        internal void ColonyLinesRequestHandler(ColonyLinesRequestMessage message)
        {
            SendColonyLinesResponseMessage();
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

        internal void SendColonyLinesResponseMessage()
        {
            var message = new ColonyLinesResponseMessage
                          {
                              Lines = m_Source.Lines
                          };

            m_MemoryBus.PublishAsync(message);

            LogLines(m_Source.Lines);
        }

        internal void TestLineResponseHandler(TestLineResponseMessage message)
        {
            m_Factory.Release(m_Source);

            m_Converter.Dtos = message.LineDtos;
            m_Converter.Convert();

            m_Source = m_Factory.Create(m_Converter.Lines);

            m_MemoryBus.PublishAsync(new ColonyLineResponseMessage());
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
    }
}