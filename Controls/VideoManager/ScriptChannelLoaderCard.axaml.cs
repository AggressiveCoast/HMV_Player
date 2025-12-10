using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace HMV_Player.Controls.VideoManager;

public partial class ScriptChannelLoaderCard : UserControl {
    public ScriptChannelLoaderCard() {
        InitializeComponent();
    }

    public static readonly StyledProperty<int> ChannelIndexProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, int>(nameof(ChannelIndex));

    public static readonly StyledProperty<string> FilePathProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, string>(nameof(FilePath),
            defaultValue: string.Empty,
            coerce: OnFilePathCoerce);

    public static readonly StyledProperty<ICommand> FileLoadCommandProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, ICommand>(nameof(FileLoadCommand));

    public static readonly StyledProperty<ICommand> ClearFileCommandProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, ICommand>(nameof(ClearFileCommand));

    public static readonly StyledProperty<string> FileStatusTextProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, string>(nameof(FileStatusText), "File Not Set");

    public static readonly StyledProperty<IBrush> FileStatusColorProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, IBrush>(nameof(FileStatusColor), Brushes.LightGray);

    public static readonly StyledProperty<string> FileNameProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, string>(nameof(FileName), string.Empty);

    public static readonly StyledProperty<bool> ShowToolTipProperty =
        AvaloniaProperty.Register<ScriptChannelLoaderCard, bool>(nameof(ShowToolTip), false);

    public int ChannelIndex {
        get => GetValue(ChannelIndexProperty);
        set => SetValue(ChannelIndexProperty, value);
    }

    public string FileStatusText {
        get => GetValue(FileStatusTextProperty);
        private set => SetValue(FileStatusTextProperty, value);
    }

    public IBrush FileStatusColor {
        get => GetValue(FileStatusColorProperty);
        private set => SetValue(FileStatusColorProperty, value);
    }

    public string FilePath {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public string FileName {
        get => GetValue(FileNameProperty);
        private set => SetValue(FileNameProperty, value);
    }

    public ICommand FileLoadCommand {
        get => GetValue(FileLoadCommandProperty);
        set => SetValue(FileLoadCommandProperty, value);
    }

    public ICommand ClearFileCommand {
        get => GetValue(ClearFileCommandProperty);
        set => SetValue(ClearFileCommandProperty, value);
    }

    public bool ShowToolTip {
        get => GetValue(ShowToolTipProperty);
        private set => SetValue(ShowToolTipProperty, value);
    }

    private static string OnFilePathCoerce(AvaloniaObject sender, string value) {
        if (sender is ScriptChannelLoaderCard card) {
            card.UpdateFileStatus(value);
        }

        return value;
    }

    private void UpdateFileStatus(string filePath) {
        if (string.IsNullOrEmpty(filePath)) {
            FileStatusText = "File Not Set";
            FileStatusColor = Brushes.LightGray;
            FileName = string.Empty;
            ShowToolTip = false;
        }
        else if (!File.Exists(filePath)) {
            FileStatusText = "File Missing";
            FileStatusColor = Brushes.DarkRed;
            FileName = Path.GetFileName(filePath);
            ShowToolTip = true;
        }
        else {
            FileStatusText = "File Loaded";
            FileStatusColor = Brushes.Green;
            FileName = Path.GetFileName(filePath);
            ShowToolTip = true;
        }
    }
}