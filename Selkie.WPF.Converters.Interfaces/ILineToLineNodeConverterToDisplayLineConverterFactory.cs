using Selkie.Windsor;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface ILineToLineNodeConverterToDisplayLineConverterFactory : ITypedFactory
    {
        ILineToLineNodeConverterToDisplayLineConverter Create();
        void Release(ILineToLineNodeConverterToDisplayLineConverter converter);
    }
}