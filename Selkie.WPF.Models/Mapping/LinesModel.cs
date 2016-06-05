using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public sealed class LinesModel
        : ILinesModel,
          IDisposable
    {
        public LinesModel([NotNull] ISelkieLogger logger,
                          [NotNull] ISelkieInMemoryBus bus,
                          [NotNull] ILinesSourceManager linesSourceManager,
                          [NotNull] IDisplayLineFactory displayLineFactory)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_LinesSourceManager = linesSourceManager;
            m_DisplayLineFactory = displayLineFactory;
            m_DisplayLines = new List <IDisplayLine>();

            LoadDisplayLines(linesSourceManager.Lines);

            bus.SubscribeAsync <ColonyLineResponseMessage>(GetType().ToString(),
                                                           ColonyLineResponsedHandler);

            bus.SubscribeAsync <LinesModelLinesRequestMessage>(GetType().ToString(),
                                                               LinesModelLinesRequestHandler);
        }

        private readonly ISelkieInMemoryBus m_Bus;
        private readonly IDisplayLineFactory m_DisplayLineFactory;
        private readonly List <IDisplayLine> m_DisplayLines;
        private readonly ILinesSourceManager m_LinesSourceManager;
        private readonly ISelkieLogger m_Logger;

        public void Dispose()
        {
            ReleaseDisplayLines();
        }

        internal void ColonyLineResponsedHandler(ColonyLineResponseMessage message)
        {
            m_Logger.Debug("Handling '{0}'...".Inject(message.GetType()));

            Update(m_LinesSourceManager.Lines);
        }

        internal void LinesModelLinesRequestHandler(LinesModelLinesRequestMessage obj)
        {
            m_Bus.PublishAsync(new LinesModelChangedMessage());
        }

        internal void Update([NotNull] IEnumerable <ILine> lines)
        {
            ReleaseDisplayLines();
            LoadDisplayLines(lines);
        }

        #region ILinesModel Members

        public IEnumerable <IDisplayLine> Lines
        {
            get
            {
                return m_DisplayLines;
            }
        }

        internal void ClearDisplayLines()
        {
            ReleaseDisplayLines();

            m_DisplayLines.Clear();
        }

        internal void ReleaseDisplayLines()
        {
            foreach ( IDisplayLine displayLine in m_DisplayLines )
            {
                m_DisplayLineFactory.Release(displayLine);
            }
        }

        internal void LoadDisplayLines(IEnumerable <ILine> lines)
        {
            m_DisplayLines.Clear();

            foreach ( ILine line in lines )
            {
                IDisplayLine displayLine = m_DisplayLineFactory.Create(line);

                m_DisplayLines.Add(displayLine);
            }

            m_Bus.PublishAsync(new LinesModelChangedMessage());
        }

        #endregion
    }
}