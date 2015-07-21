using Selkie.Windsor;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IConverterFactory : ITypedFactory
    {
        T Create <T>() where T : IConverter;
        void Release(IConverter converter);
    }
}