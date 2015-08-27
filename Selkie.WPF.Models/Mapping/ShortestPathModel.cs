using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Framework.Common.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Converters.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces.Mapping;

namespace Selkie.WPF.Models.Mapping
{
    public sealed class ShortestPathModel
        : IShortestPathModel,
          IDisposable
    {
        private readonly ISelkieInMemoryBus m_MemoryBus;
        private readonly ILineToLineNodeConverterToDisplayLineConverterFactory m_Factory;
        private readonly ISelkieLogger m_Logger;
        private readonly IPathToLineToLineNodeConverter m_PathToLineToLineNodeConverter;
        private ILineToLineNodeConverterToDisplayLineConverter m_Converter;
        private IEnumerable <ILineToLineNodeConverter> m_Nodes;

        public ShortestPathModel([NotNull] ISelkieLogger logger,
                                 [NotNull] ISelkieBus bus,
                                 [NotNull] ISelkieInMemoryBus memoryBus,
                                 [NotNull] IPathToLineToLineNodeConverter pathToLineToLineNodeConverter,
                                 [NotNull] ILineToLineNodeConverterToDisplayLineConverterFactory factory)
        {
            m_Logger = logger;
            m_MemoryBus = memoryBus;
            m_PathToLineToLineNodeConverter = pathToLineToLineNodeConverter;
            m_Factory = factory;
            m_Nodes = new ILineToLineNodeConverter[0];
            m_Converter = m_Factory.Create();

            bus.SubscribeAsync <ColonyBestTrailMessage>(GetType().FullName,
                                                        ColonyBestTrailHandler);
        }

        public void Dispose()
        {
            m_Factory.Release(m_Converter);
        }

        public IEnumerable <ILineToLineNodeConverter> Nodes
        {
            get
            {
                lock ( this )
                {
                    return m_Nodes;
                }
            }
        }

        public IEnumerable <IDisplayLine> Path
        {
            get
            {
                return m_Converter.DisplayLines;
            }
        }

        internal void ColonyBestTrailHandler(ColonyBestTrailMessage message)
        {
            m_Logger.Debug("Handling '{0}'...".Inject(message.GetType()));

            Update(message);
        }

        internal void Update(ColonyBestTrailMessage message)
        {
            lock ( this )
            {
                ConvertPath(message.Trail);

                UpdateNodes();
                UpdateConverter();
            }

            m_MemoryBus.Publish(new ShortestPathModelChangedMessage());
        }

        internal void ConvertPath(IEnumerable <int> trail)
        {
            m_PathToLineToLineNodeConverter.Path = trail;
            m_PathToLineToLineNodeConverter.Convert();
        }

        internal void UpdateNodes()
        {
            m_Nodes = m_PathToLineToLineNodeConverter.Nodes;
        }

        internal void UpdateConverter()
        {
            ILineToLineNodeConverterToDisplayLineConverter oldConverter = m_Converter;

            ILineToLineNodeConverterToDisplayLineConverter converter = m_Factory.Create();
            converter.Converters = m_Nodes;
            converter.Convert();

            m_Converter = converter;

            m_Factory.Release(oldConverter);
        }
    }
}