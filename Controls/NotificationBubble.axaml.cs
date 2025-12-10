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

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<NotificationBubble, ICommand?>(nameof(CloseCommand));

    public static readonly StyledProperty<object?> CloseCommandParameterProperty =
        AvaloniaProperty.Register<NotificationBubble, object?>(nameof(CloseCommandParameter));

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

    public ICommand? CloseCommand {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public object? CloseCommandParameter {
        get => GetValue(CloseCommandParameterProperty);
        set => SetValue(CloseCommandParameterProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        if (closeButton != null) {
            closeButton.Click += (s, args) => {
                if (CloseCommand?.CanExecute(CloseCommandParameter) == true) {
                    CloseCommand.Execute(CloseCommandParameter);
                }
            };
        }
    }
}

public enum NotificationType {
    Info,
    Success,
    Warning,
    Error
}