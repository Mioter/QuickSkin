using System;
using Avalonia.Controls.Notifications;

namespace QuickSkin.Models;

public class NotificationItem
{
    public required string Title { get; set; }
    
    public required string Message { get; set; }

    public NotificationType Type { get; set; } = NotificationType.Information;

    public required DateTime Time { get; set; }
}
