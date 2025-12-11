using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HMV_Player.Services.Storage;

namespace HMV_Player.MVVM.Views;

public partial class MainView : Window {
    private readonly UserSettingsStorageService _userSettingsStorageService;
    private bool _isInitialized = false;
    public MainView(UserSettingsStorageService userSettingsStorageService) {
        InitializeComponent();
        _userSettingsStorageService = userSettingsStorageService;
        if (_userSettingsStorageService.DataInstance.IsMaximized) {
            WindowState = WindowState.Maximized;
        }
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        PropertyChanged += OnWindowPropertyChanged;
        
        Avalonia.Threading.Dispatcher.UIThread.Post(() => 
        {
            _isInitialized = true;
        }, Avalonia.Threading.DispatcherPriority.Loaded);
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
    
    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_isInitialized) return;
        else if (e.Property == WindowStateProperty)
        {
            _userSettingsStorageService.DataInstance.WindowState = WindowState;
            _userSettingsStorageService.DataInstance.IsMaximized = WindowState == WindowState.Maximized;
        }
    }
}