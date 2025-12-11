using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HMV_Player.Data;
using HMV_Player.Factories;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.Devices;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.Services;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.ButtplugIo;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.TheHandy;
using HMV_Player.Services.DialogueWindow;
using HMV_Player.Services.Funscript;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;
using HMV_Player.Services.VideoLibrary;
using HMV_Player.Services.VideoPlayer;
using Microsoft.Extensions.DependencyInjection;

namespace HMV_Player;

public static class DependencyInjection {
    public static void SetupDependencyInjection(ServiceCollection services, IApplicationLifetime applicationLifetime) {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<DevicesViewModel>();
        services.AddTransient<PlayVideoViewModel>();
        services.AddTransient<VideoManagerViewModel>();

        services.AddSingleton<ToyScriptPlayerService>();

        services.AddSingleton<VideoPlayerViewModel>();


        initializeServices(services);
        initializeDevicePages(services);
        initializeFactories(services);
        initializeDeviceControllers(services);

        services.AddSingleton<NotificationContainerViewModel>();
        //TopLevel Provider
        services.AddSingleton<Func<TopLevel?>>(x => () => {
            if (applicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow) {
                return TopLevel.GetTopLevel(topWindow.MainWindow);
            }

            return null;
        });
    }

    private static void initializeDevicePages(ServiceCollection collection) {
        collection.AddTransient<LovensePageViewModel>();
        collection.AddTransient<NogasmPageViewModel>();
        collection.AddTransient<TheHandyPageViewModel>();
        collection.AddTransient<ButtplugIoViewModel>();
    }


    private static void initializeFactories(ServiceCollection collection) {
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
            DeviceBrands.TheHandy => x.GetRequiredService<TheHandyPageViewModel>(),
            DeviceBrands.ButtplugIo => x.GetRequiredService<ButtplugIoViewModel>(),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        });

        collection.AddSingleton<DevicesPageFactory>();
    }

    private static void initializeServices(ServiceCollection collection) {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            collection.AddSingleton<IThumbnailExtractor, WindowsThumbNailExtractorService>();
        }
        else {
            throw new PlatformNotSupportedException(RuntimeInformation.OSDescription);
        }


        collection.AddTransient<NogasmAnalyzerService>();
        collection.AddSingleton<ILovenseApiService, LovenseApiService>();
        collection.AddTransient<TheHandyApiService>();
        collection.AddSingleton<IButtplugIoClientConnectionService, ButtplugIoClientConnectionService>();

        collection.AddSingleton<IVideoPlayer, VlcVideoPlayerService>();

        collection.AddSingleton<VideoDataStorageService>();
        collection.AddSingleton<UserSettingsStorageService>();
        collection.AddSingleton<EdgeToyInterceptorStorageService>();
        collection.AddSingleton<TheHandySettingsStorageService>();
        collection.AddSingleton<LovenseToySettingsStorageService>();

        collection.AddSingleton<IDialogueService, DialogueService>();

        collection.AddSingleton<FunscriptPlayerService>();
    }

    private static void initializeDeviceControllers(ServiceCollection collection) {
        collection.AddTransient<LovenseDeviceController>();
        collection.AddSingleton<EdgeToyInterceptorService>();
        collection.AddTransient<TheHandyDeviceController>();
    }
}