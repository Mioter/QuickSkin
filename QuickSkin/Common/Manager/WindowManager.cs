using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using QuickSkin.Definitions.ClassBase;
using QuickSkin.Views;

namespace QuickSkin.Common.Manager;

public static class WindowManager
{
    public static InteractiveWindowBase? TopLevel { get; private set; }

    public static void OpenWorkspace(string name)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;
        var mainWindow = new WorkWindow();
        mainWindow.Show();

        TopLevel?.CloseWindow();
        desktop.MainWindow = TopLevel = mainWindow;
    }

    public static void OpenGuideWindow(IClassicDesktopStyleApplicationLifetime? desktop = null)
    {
        desktop ??= Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        if (desktop is null)
            return;

        var loginWindow = new GuideWindow();
        loginWindow.Show();
        TopLevel?.CloseWindow();

        desktop.MainWindow = TopLevel = loginWindow;
    }
}
