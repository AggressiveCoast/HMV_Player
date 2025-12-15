using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HMV_Player.Controls;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Funscript;
using HMV_Player.Services.VideoPlayer;

namespace HMV_Player.Services.Devices;

public class ToyScriptPlayerService {
    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;
    private readonly LovenseDeviceController _lovenseController;
    private readonly ToyDataManager _toyDataManager;
    private VideoFileData _videoFileData;

    private FunscriptPlayer? _funscriptPlayerChannel1;
    private FunscriptPlayer? _funscriptPlayerChannel2;
    private FunscriptPlayer? _funscriptPlayerChannel3;

    private int _script1LastIndex;
    private int _script2LastIndex;
    private int _script3LastIndex;

    private bool _isVideoPlaying;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _processingTask;
    private IVideoPlayer _videoPlayer;

    public ToyScriptPlayerService(VideoPlayerViewModel videoPlayerViewModel, LovenseDeviceController lovenseController,
        EdgeToyInterceptorService edgeToyInterceptorService, IVideoPlayer videoPlayer,
        ToyDataManager toyDataManager) {
        _lovenseController = lovenseController;
        _toyDataManager = toyDataManager;
        _videoPlayer = videoPlayer;
        _edgeToyInterceptorService = edgeToyInterceptorService;
        videoPlayerViewModel.OnMediaLoaded += player => { PrimeProcessesing(); };
        videoPlayerViewModel.OnPausedAction += player => { PauseProcessing(); };
        videoPlayerViewModel.OnResumeAction += player => { ResumeProcessing(); };
        videoPlayerViewModel.OnEndedAction += StopProcessing;
    }

    private void PrimeProcessesing() {
        Console.WriteLine($"Started");
        var funscriptFile1 = LoadFunScript(_videoFileData.FunscriptChannel1FileLocation);
        if (funscriptFile1 != null) {
            _funscriptPlayerChannel1 = new FunscriptPlayer(FunScriptChannel.Channel1, funscriptFile1,
                _lovenseController, _edgeToyInterceptorService, _toyDataManager);
        }

        var funscriptFile2 = LoadFunScript(_videoFileData.FunscriptChannel2FileLocation);
        if (funscriptFile2 != null) {
            _funscriptPlayerChannel2 = new FunscriptPlayer(FunScriptChannel.Channel2, funscriptFile2,
                _lovenseController, _edgeToyInterceptorService, _toyDataManager);
        }

        var funscriptFile3 = LoadFunScript(_videoFileData.FunscriptChannel3FileLocation);
        if (funscriptFile3 != null) {
            _funscriptPlayerChannel3 = new FunscriptPlayer(FunScriptChannel.Channel3, funscriptFile3,
                _lovenseController, _edgeToyInterceptorService, _toyDataManager);
        }
    }


    private void PauseProcessing() {
        Console.WriteLine($"Paused");
        _isVideoPlaying = false;
        _funscriptPlayerChannel1?.PauseDevices();
        _funscriptPlayerChannel2?.PauseDevices();
        _funscriptPlayerChannel3?.PauseDevices();
        StopProcessing();
    }

    private void ResumeProcessing() {
        Console.WriteLine($"Resumed");
        _isVideoPlaying = true;
        StartProcessing();
    }

    private void StartProcessing() {
        _edgeToyInterceptorService.StartInterceptorTracking();
        _cancellationTokenSource = new CancellationTokenSource();
        _processingTask = processScriptsTask(_cancellationTokenSource.Token);
    }

    private void StopProcessing() {
        _ = stopProcessingTask();
        _edgeToyInterceptorService.StopInterceptorTracking();
    }

    private async Task processScriptsTask(CancellationToken cancellationToken) {
        try {
            while (true) {
                cancellationToken.ThrowIfCancellationRequested();
                long currentVideoTime = _videoPlayer.Player.Time;
                _funscriptPlayerChannel1?.ProcessFunscript(currentVideoTime, _isVideoPlaying,
                    _cancellationTokenSource.Token);
                _funscriptPlayerChannel2?.ProcessFunscript(currentVideoTime, _isVideoPlaying,
                    _cancellationTokenSource.Token);
                _funscriptPlayerChannel3?.ProcessFunscript(currentVideoTime, _isVideoPlaying,
                    _cancellationTokenSource.Token);

                await Task.Delay(100, cancellationToken);
            }
        }
        catch (OperationCanceledException) {
            // do nothing, just exit
        }
        catch (Exception ex) {
            ErrorLogger.Log($"Unhandled exception in processScripts: {ex.Message}");
        }
    }

    private FunscriptFile? LoadFunScript(string path) {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }

        try {
            if (File.Exists(path)) {
                var json = File.ReadAllText(path);
                var instance = JsonSerializer.Deserialize<FunscriptFile>(json);
                NotificationService.ShowNotification("Funscript loaded", path,
                    NotificationType.Info);
                return instance;
            }

            NotificationService.ShowNotification("Unable to Find Funscript", path,
                NotificationType.Warning);
        }
        catch (Exception e) {
            NotificationService.ShowNotification("Unable to Load Funscript", path, NotificationType.Error);
            ErrorLogger.Log(e.Message);
        }

        return null;
    }


    public void SetVideoFileData(VideoFileData videoFileData) {
        _videoFileData = videoFileData;
    }

    private async Task stopProcessingTaskAsync() {
        if (_processingTask != null && !_processingTask.IsCompleted) {
            _cancellationTokenSource?.Cancel();

            try {
                await _processingTask;
            }
            catch (OperationCanceledException) {
            }
            catch (Exception ex) {
                ErrorLogger.Log(ex);
            }
        }

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _processingTask = null;
    }

// Event handler needs to be updated to call the async method
    private async Task stopProcessingTask() {
        await stopProcessingTaskAsync();
        _edgeToyInterceptorService.StopInterceptorTracking();
    }
}