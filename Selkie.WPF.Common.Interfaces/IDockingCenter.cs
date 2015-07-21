using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IDockingCenter
    {
        void SetManager(object manager);

        void AssignToArea(IView view,
                          string title);
    }
}