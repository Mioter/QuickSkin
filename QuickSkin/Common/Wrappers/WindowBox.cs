using System.Threading.Tasks;
using Avalonia.Controls;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Models;
using Ursa.Controls;

namespace QuickSkin.Common.Wrappers;

public static class WindowBox
{
    public static async Task<TResult?> ShowDialog<TResult>(
        object content,
        IDialogContext model,
        ShowWindowOptions options,
        UrsaWindow toolLevel
        )
    {
        TResult? finalResult = default;

        var window = new UrsaWindow
        {
            DataContext = model,
            Content = content,
            Title = options.Title,
            CanResize = options.CanResize,
            SizeToContent = options.SizeToContent,
            WindowStartupLocation = options.StartupLocation,
            IsCloseButtonVisible = options.IsCloseButtonVisible,
            IsMinimizeButtonVisible = options.IsMinimizeButtonVisible,
            IsRestoreButtonVisible = options.IsRestoreButtonVisible,
            IsFullScreenButtonVisible = options.IsFullScreenButtonVisible,
            MaxWidth = options.MaxWidth,
            MaxHeight = options.MaxHeight,
            MinWidth = options.MinWidth,
            MinHeight = options.MinHeight,
        };

        model.RequestClose += WindowDateOnRequestClose;
        await window.ShowDialog(toolLevel);
        model.RequestClose -= WindowDateOnRequestClose;

        return finalResult;

        void WindowDateOnRequestClose(object? sender, object? e)
        {
            finalResult = e is TResult result ? result : default;
            window.Close();
        }
    }

    public static Task<TResult?> ShowDialog<TControl, TResult>(
        IDialogContext model,
        ShowWindowOptions options,
        UrsaWindow toolLevel
        )
        where TControl : Control, new()
    {
        return ShowDialog<TResult>(new TControl(), model, options, toolLevel);
    }
}
