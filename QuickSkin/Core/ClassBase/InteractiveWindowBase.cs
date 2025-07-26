using Avalonia.Controls;
using QuickSkin.Common.Services;
using Ursa.Controls;

namespace QuickSkin.Core.ClassBase;

public class InteractiveWindowBase : UrsaWindow
{
    public void ShowWindow()
    {
        if (IsVisible)
        {
            Activate();
            Topmost = true;
            Topmost = false;

            NotificationService.Info("看我", "窗口已经在显示了~");

            return;
        }

        Show();
        Activate();
        WindowState = WindowState.Normal;
    }

    public virtual void CloseWindow() { }
}
