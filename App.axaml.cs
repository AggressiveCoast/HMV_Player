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
using HMV_Player.Services;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Lovense;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.DialogueWindow;
using HMV_Player.Services.Storage;
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

        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<HomePageViewModel>();
        collection.AddTransient<DevicesViewModel>();
        collection.AddTransient<PlayVideoViewModel>();
        collection.AddTransient<VideoManagerViewModel>();


        initializeServices(collection);
        initializeDevicePages(collection);
        initializeFactories(collection);

        collection.AddSingleton<ToysProcessorService>();

        //TopLevel Provider
        collection.AddSingleton<Func<TopLevel?>>(x => () => {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow) {
                return TopLevel.GetTopLevel(topWindow.MainWindow);
            }

            return null;
        });


        initializeApp(collection);

        base.OnFrameworkInitializationCompleted();
    }

    private void initializeApp(ServiceCollection services) {
        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainView() {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
        }
    }

    private void initializeDevicePages(ServiceCollection collection) {
        collection.AddSingleton<LovensePageViewModel>();
        collection.AddSingleton<NogasmPageViewModel>();
    }


    private void initializeFactories(ServiceCollection collection) {
        collection.AddSingleton<Func<ApplicationPageName, PageViewModel>>(x =>
            name => name switch {
                ApplicationPageName.Home => x.GetRequiredService<HomePageViewModel>(),
                ApplicationPageName.PlayVideo => x.GetRequiredService<PlayVideoViewModel>(),
                ApplicationPageName.Devices => x.GetRequiredService<DevicesViewModel>(),
                ApplicationPageName.VideoManager => x.GetRequiredService<VideoManagerViewModel>(),
                _ => throw new InvalidOperationException(),
            });

        collection.AddSingleton<PageFactory>();

        collection.AddSingleton<Func<DeviceBrands, DevicesPageViewModel>>(x => name => name switch {
            DeviceBrands.Lovense => x.GetRequiredService<LovensePageViewModel>(),
            DeviceBrands.Nogasm => x.GetRequiredService<NogasmPageViewModel>(),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        });

        collection.AddSingleton<DevicesPageFactory>();
    }

    private void initializeServices(ServiceCollection collection) {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            collection.AddSingleton<IThumbnailExtractor, WindowsThumbNailExtractorService>();
        }

        collection.AddSingleton<NogasmAnalyzerService>();
        collection.AddSingleton<ILovenseApiService, LovenseApiService>();

        collection.AddSingleton<IVideoPlayer, VlcVideoPlayerService>();

        collection.AddSingleton<ToyScriptProcessorsStorageService>();
        collection.AddSingleton<VideoDataStorageService>();

        collection.AddSingleton<IDialogueService, DialogueService>();
    }
}