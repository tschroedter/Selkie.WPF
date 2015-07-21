using Selkie.Windsor;

namespace Selkie.WPF.Common.Interfaces.Windsor
{
    public interface IViewFactory : ITypedFactory
    {
        T CreateView <T>() where T : IView;
        T CreateView <T>(object argumentsAsAnonymousType) where T : IView;
    }
}