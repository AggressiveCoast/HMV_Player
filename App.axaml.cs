using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using HMV_Player.Data;
using HMV_Player.Factories;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.Devices;
using HMV_Player.MVVM.ViewModels.VideoManager;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.Services;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.DialogueWindow;
using HMV_Player.Services.Funscript;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;
using HMV_Player.Services.VideoLibrary;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "HMV_Player.Controls")]

namespace HMV_Player;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {

        

        var collection = new ServiceCollection();

        DependencyInjection.SetupDependencyInjection(collection, ApplicationLifetime);
        
        initializeApp(collection);

        base.OnFrameworkInitializationCompleted();
    }

    private void initializeApp(ServiceCollection services) {
        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MVVM.Views.MainView(provider.GetRequiredService<UserSettingsStorageService>()) {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
        }
    }
}