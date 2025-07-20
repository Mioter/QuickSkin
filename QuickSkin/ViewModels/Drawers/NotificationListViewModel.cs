using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common.Services;
using QuickSkin.Models;

namespace QuickSkin.ViewModels.Drawers;

public partial class NotificationListViewModel : ObservableObject
{
    public AvaloniaList<NotificationItem> NotificationItems { get; } = NotificationService.Notifications;

    [RelayCommand]
    private void RemoveNotification(NotificationItem notificationItem)
    {
        NotificationItems.Remove(notificationItem);
    }
}
