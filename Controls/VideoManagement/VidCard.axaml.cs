using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace HMV_Player.Controls.VideoManagement;

public class VidCard : Button {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<VidCard, string>(nameof(Title));

    public static readonly StyledProperty<string> ThumbnailUrlProperty =
        AvaloniaProperty.Register<VidCard, string>(nameof(ThumbnailUrl));
    
    public static readonly StyledProperty<Bitmap?> ThumbnailImageProperty =
        AvaloniaProperty.Register<VidCard, Bitmap?>(nameof(ThumbnailImage));

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string ThumbnailUrl {
        get => GetValue(ThumbnailUrlProperty);
        set => SetValue(ThumbnailUrlProperty, value);
    }

    public Bitmap? ThumbnailImage {
        get => GetValue(ThumbnailImageProperty);
        set => SetValue(ThumbnailImageProperty, value);
    }
}