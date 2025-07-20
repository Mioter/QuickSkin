using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Core.ClassBase;
using QuickSkin.Views.Drawers;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace QuickSkin.ViewModels;

public partial class WorkWindowViewModel : ViewModelBase
{
    public string Title { get; } =
        $"{Workplace.CurrentAssemblyName} • {Workplace.WorkspaceInfo?.Name ?? "未选择工作区"}";
    
    
    [RelayCommand]
    private static void CloseWorkspace()
    {
        WindowManager.OpenGuideWindow();
    }

    [RelayCommand]
    private void ShowNotificationList()
    {
        var options = new DrawerOptions
        {
            Position = Position.Right,
            Buttons = DialogButton.None,
            CanLightDismiss = true,
            IsCloseButtonVisible = true,
            Title = "信息列表",
            MinWidth = 300,
        };

        Drawer.Show(new NotificationList(), null, null, options);
    }
}
