using System;
using System.Text;
using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.WPF.Models.Handlers
{
    [ProjectComponent(Lifestyle.Transient)]
    public class ExceptionThrownMessageToStringConverter : IExceptionThrownMessageToStringConverter
    {
        public string Convert(ExceptionThrownMessage message)
        {
            string text = ExceptionInformationToString(message.Exception);

            text += InnerException(message.InnerExceptions);

            return text;
        }

        private static string InnerException(ExceptionInformation[] informationArray)
        {
            if ( informationArray == null )
            {
                return string.Empty;
            }

            string innerExceptions = string.Empty;

            foreach ( ExceptionInformation information in informationArray )
            {
                innerExceptions += "Inner Exception:" + Environment.NewLine;
                innerExceptions += ExceptionInformationToString(information);
            }

            return innerExceptions;
        }

        private static string ExceptionInformationToString([NotNull] ExceptionInformation information)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Invocation: {0}".Inject(information.Invocation));
            builder.AppendLine("Message: {0}".Inject(information.Message));
            builder.AppendLine("StackTrace: {0}".Inject(information.StackTrace));

            return builder.ToString();
        }
    }
}