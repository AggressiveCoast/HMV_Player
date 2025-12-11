using System;
using System.IO;
using System.Text.Json;
using HMV_Player.Controls;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.VideoPlayer;

namespace HMV_Player.Services.Devices;

public class ToyScriptPlayerService {
    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;
    private readonly NotificationContainerViewModel _notificationContainerViewModel;
    private VideoFileData _videoFileData;

    private FunscriptWrapper _funscriptChannel1;
    private FunscriptWrapper _funscriptChannel2;
    private FunscriptWrapper _funscriptChannel3;


    public ToyScriptPlayerService(VideoPlayerViewModel videoPlayerViewModel,
        EdgeToyInterceptorService edgeToyInterceptorService,
        NotificationContainerViewModel notificationContainerViewModel) {
        _edgeToyInterceptorService = edgeToyInterceptorService;
        _notificationContainerViewModel = notificationContainerViewModel;
        videoPlayerViewModel.OnMediaLoaded += player => { PrimeProcessesing(); };
        videoPlayerViewModel.OnPausedAction += player => { PauseProcessing(); };
        videoPlayerViewModel.OnResumeAction += player => { ResumeProcessing(player.Time); };
        videoPlayerViewModel.OnEndedAction += StopProcessing;
    }

    private void PrimeProcessesing() {
        Console.WriteLine($"Started");
        var funscriptFile1 = LoadFunScript(_videoFileData.FunscriptChannel1FileLocation);
        if (funscriptFile1 != null) {
            _funscriptChannel1 = new FunscriptWrapper(funscriptFile1);
        }
        
        var funscriptFile2 = LoadFunScript(_videoFileData.FunscriptChannel2FileLocation);
        if (funscriptFile2 != null) {
            _funscriptChannel2 = new FunscriptWrapper(funscriptFile2);
        }
        
        var funscriptFile3 = LoadFunScript(_videoFileData.FunscriptChannel3FileLocation);
        if (funscriptFile3 != null) {
            _funscriptChannel3 = new FunscriptWrapper(funscriptFile3);
        }

        _edgeToyInterceptorService.StartInterceptorTracking();
    }

    private void PauseProcessing() {
        Console.WriteLine($"Paused");
    }

    private void ResumeProcessing(long timeInMs) {
        Console.WriteLine($"Resumed");
    }

    private void StopProcessing() {
        Console.WriteLine($"Stop");
        _edgeToyInterceptorService.StopInterceptorTracking();
    }

    private FunscriptFile? LoadFunScript(string path) {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }

        try {
            if (File.Exists(path)) {
                var json = File.ReadAllText(path);
                var instance = JsonSerializer.Deserialize<FunscriptFile>(json);
                _notificationContainerViewModel.ShowNotification("Funscript loaded", path,
                    NotificationType.Info);
                return instance;
            }

            _notificationContainerViewModel.ShowNotification("Unable to Find Funscript", path,
                NotificationType.Warning);
        }
        catch (Exception e) {
            _notificationContainerViewModel.ShowNotification("Unable to Load Funscript", path, NotificationType.Error);
            ErrorLogger.Log(e.Message);
        }

        return null;
    }


    public void SetVideoFileData(VideoFileData videoFileData) {
        _videoFileData = videoFileData;
    }
}