using Avalonia.Controls;
using Avalonia.Interactivity;
using HMV_Player.Services.Storage;

namespace HMV_Player.MVVM.Views;

public partial class MainView : Window {
    private readonly UserSettingsStorageService _userSettingsStorageService;

    public MainView(UserSettingsStorageService userSettingsStorageService) {
        InitializeComponent();
        _userSettingsStorageService = userSettingsStorageService;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e) {
        _userSettingsStorageService.DataInstance.UserWindowWidth = Width;
        _userSettingsStorageService.DataInstance.UserWindowHeight = Height;
        _userSettingsStorageService.Save();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e) {
        Width = _userSettingsStorageService.DataInstance.UserWindowWidth;
        Height = _userSettingsStorageService.DataInstance.UserWindowHeight;
    }
}