using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.WPF.Models.Handlers
{
    [Interceptor(typeof ( MessageHandlerAspect ))]
    public class ExceptionThrownHandler
        : SelkieMessageHandler <ExceptionThrownMessage>
    {
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly IExceptionThrownMessageToStringConverter m_Converter;
        private readonly ISelkieLogger m_Logger;

        public ExceptionThrownHandler([NotNull] ISelkieLogger logger,
                                      [NotNull] ISelkieInMemoryBus bus,
                                      [NotNull] IExceptionThrownMessageToStringConverter converter)
        {
            m_Logger = logger;
            m_Bus = bus;
            m_Converter = converter;
        }

        public override void Handle(ExceptionThrownMessage message)
        {
            m_Logger.Error(m_Converter.Convert(message));

            string statusText = "Error: {0}".Inject(message.Exception.Message);

            m_Bus.PublishAsync(new StatusMessage
                               {
                                   Text = statusText
                               });
        }
    }
}