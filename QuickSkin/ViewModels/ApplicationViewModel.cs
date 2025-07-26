using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common.Manager;
using QuickSkin.Models.Configs;
using Serilog;

namespace QuickSkin.ViewModels;

public partial class ApplicationViewModel : ObservableObject
{
    public UiConfig UiConfig { get; set; } = ConfigManager.ConfigModel.UiConfig;

    [RelayCommand]
    private static void ShowMainWindow()
    {
        if (WindowManager.TopLevel is not { } window)
            return;

        window.ShowWindow();
    }

    [RelayCommand]
    private static void ExitApplication()
    {
        if (WindowManager.TopLevel is { } window)
        {
            window.CloseWindow();
        }

        Shutdown();
    }

    public static void Shutdown()
    {
        try
        {
            ConfigManager.Save();

            Log.Information(
                "\n"
              + "===========================================\n"
              + "应用程序已退出\n"
              + "===========================================\n"
            );

            Log.CloseAndFlush();

            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;

            desktop.Shutdown();
        }
        catch (Exception ex)
        {
            Log.Error("程序退出时发生错误: {Message}", ex.Message);
            Log.CloseAndFlush();
        }
    }
}
