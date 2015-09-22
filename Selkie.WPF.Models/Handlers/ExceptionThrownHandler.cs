using System.Text;
using Castle.Core;
using JetBrains.Annotations;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.WPF.Models.Handlers
{
    // todo first implementation, just loggin exceptions coming from the bus
    [Interceptor(typeof(MessageHandlerAspect))]
    public class ExceptionThrownHandler
        : SelkieMessageHandler<ExceptionThrownMessage>
    {
        private readonly ISelkieLogger m_Logger;

        public ExceptionThrownHandler([NotNull] ISelkieLogger logger)
        {
            m_Logger = logger;
        }

        public override void Handle(ExceptionThrownMessage message)
        {
            m_Logger.Error(MessageToText(message));
        }

        private string MessageToText(ExceptionThrownMessage message)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Invocation: {0}".Inject(message.Invocation));
            builder.AppendLine("Message: {0}".Inject(message.Message));
            builder.AppendLine("StackTrace: {0}".Inject(message.StackTrace));

            return builder.ToString();
        }
    }
}