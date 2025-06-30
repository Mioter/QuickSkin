using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common.Manager;

namespace QuickSkin.ViewModels;

public partial class GuideWindowViewModel : ViewModelBase
{
    [RelayCommand]
    private static void OpenWorkspace()
    {
        WindowManager.OpenWorkspace("Workspace");
    }
}
