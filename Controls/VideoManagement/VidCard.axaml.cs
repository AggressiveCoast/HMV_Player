using Avalonia;
using Avalonia.Controls;

namespace HMV_Player.Controls.VideoManagement;

public class VidCard : Button {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<VidCard, string>(nameof(Title));

    public static readonly StyledProperty<string> ThumbnailUrlProperty =
        AvaloniaProperty.Register<VidCard, string>(nameof(ThumbnailUrl));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string ThumbnailUrl
    {
        get => GetValue(ThumbnailUrlProperty);
        set => SetValue(ThumbnailUrlProperty, value);
    }
}