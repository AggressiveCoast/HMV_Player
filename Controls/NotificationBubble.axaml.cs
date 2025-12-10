using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace HMV_Player.Controls;

public class NotificationBubble : TemplatedControl {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<NotificationBubble, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<NotificationBubble, string>(nameof(Message), string.Empty);

    public static readonly StyledProperty<NotificationType> NotificationTypeProperty =
        AvaloniaProperty.Register<NotificationBubble, NotificationType>(nameof(NotificationType),
            NotificationType.Info);

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public NotificationType NotificationType {
        get => GetValue(NotificationTypeProperty);
        set => SetValue(NotificationTypeProperty, value);
    }
}

public enum NotificationType {
    Info,
    Success,
    Warning,
    Error
}