using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common.Manager;

namespace QuickSkin.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NavigationWidth))]
    public partial bool IsNavigationExpand { get; set; }

    public double NavigationWidth => IsNavigationExpand ? 130 : 55;

    [RelayCommand]
    private static void CloseWorkspace()
    {
        WindowManager.OpenGuideWindow();
    }
}
