using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using QuickSkin.Common.Services;
using Ursa.Controls;

namespace QuickSkin.Definitions.ClassBase;

public class InteractiveWindowBase : UrsaWindow
{
    public void ShowWindow()
    {
        if (IsVisible)
        {
            Activate();
            Topmost = true;
            Topmost = false;

            NotificationService.ShowLight("看我", "窗口已经在显示了~", NotificationType.Information);
            return;
        }

        Show();
        Activate();
        WindowState = WindowState.Normal;
    }

    public virtual void CloseWindow() { }
}
