using System.Threading.Tasks;
using Avalonia.Controls;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Models;
using Ursa.Controls;

namespace QuickSkin.Common.Wrappers;

public class WindowBox
{
    private object? _result;
    private UrsaWindow? _window;

    public async Task<TResult?> ShowDialog<TResult>(object content, IDialogContext model, ShowWindowOptions options, UrsaWindow toolLevel)
    {
        _window = new UrsaWindow
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
        await _window.ShowDialog(toolLevel);
        model.RequestClose -= WindowDateOnRequestClose;

        return _result is TResult result ? result : default;
    }

    public Task<TResult?> ShowDialog<TControl, TResult>(IDialogContext model, ShowWindowOptions options, UrsaWindow toolLevel) where TControl : Control, new()
    {
        return ShowDialog<TResult>(new TControl(), model, options, toolLevel);
    }

    private void WindowDateOnRequestClose(object? sender, object? e)
    {
        _result = e;
        _window?.Close();
    }
}
