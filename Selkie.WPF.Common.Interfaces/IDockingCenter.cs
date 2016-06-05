using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Common.Interfaces
{
    public interface IDockingCenter
    {
        void AssignToArea(IView view,
                          string title);

        void SetManager(object manager);
    }
}