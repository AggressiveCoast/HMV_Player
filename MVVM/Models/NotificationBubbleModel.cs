using HMV_Player.Controls;

namespace HMV_Player.MVVM.Models;

public class NotificationBubbleModel {
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
}