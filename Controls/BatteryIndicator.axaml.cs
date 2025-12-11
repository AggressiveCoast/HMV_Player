using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg;

namespace HMV_Player.Controls;

public partial class BatteryIndicator : UserControl {
    public BatteryIndicator() {
        InitializeComponent();
        PropertyChanged += OnPropertyChanged;

    }

    public static readonly StyledProperty<int> BatteryLevelProperty =
        AvaloniaProperty.Register<BatteryIndicator, int>(
            nameof(BatteryLevel),
            defaultValue: 0,
            coerce: CoerceBatteryLevel);

    public int BatteryLevel {
        get => GetValue(BatteryLevelProperty);
        set => SetValue(BatteryLevelProperty, value);
    }

    private static int CoerceBatteryLevel(AvaloniaObject obj, int value) {
        return Math.Max(0, Math.Min(100, value));
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == BatteryLevelProperty) {
            UpdateBatteryImage();
        }
    }

    private void UpdateBatteryImage() {
        var image = this.FindControl<Image>("BatteryDisplayImage");
        if (image == null) return;

        string imagePath = GetBatteryImagePath(BatteryLevel);

        try
        {
            var svgSource = SvgSource.Load(imagePath, null);
            var svgImage = new SvgImage { Source = svgSource };
            image.Source = svgImage;
            
        }catch (Exception e) {
            image.Source = null;
        }
    }

    private string GetBatteryImagePath(int level) {
        if (level >= 90)
            return "avares://HMV%20Player/Assets/Images/Icons/phosphor/fill/battery-full-fill.svg";
        else if (level >= 60)
            return "avares://HMV%20Player/Assets/Images/Icons/phosphor/fill/battery-high-fill.svg";
        else if (level >= 25)
            return "avares://HMV%20Player/Assets/Images/Icons/phosphor/fill/battery-medium-fill.svg";
        else if (level >= 5)
            return "avares://HMV%20Player/Assets/Images/Icons/phosphor/fill/battery-low-fill.svg";
        else
            return "avares://HMV%20Player/Assets/Images/Icons/phosphor/fill/battery-empty-fill.svg";
    }
}