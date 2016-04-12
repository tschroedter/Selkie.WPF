using Selkie.Aop.Messages;

namespace Selkie.WPF.Models.Handlers
{
    public interface IExceptionThrownMessageToStringConverter
    {
        string Convert(ExceptionThrownMessage message);
    }
}