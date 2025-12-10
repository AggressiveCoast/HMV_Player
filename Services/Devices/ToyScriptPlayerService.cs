using System;
using HMV_Player.MVVM.ViewModels.VideoPlayer;

namespace HMV_Player.Services.Devices;

public class ToyScriptPlayerService {

    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;
    public ToyScriptPlayerService(VideoPlayerViewModel videoPlayerViewModel, EdgeToyInterceptorService edgeToyInterceptorService) {
        _edgeToyInterceptorService = edgeToyInterceptorService;
        videoPlayerViewModel.OnMediaLoaded += player => {
            PrimeProcessesing();
        };
        videoPlayerViewModel.OnPausedAction += player => {
            if (!player.IsPlaying) return;
            PauseProcessing();
        };
        videoPlayerViewModel.OnResumeAction += player => {
            if (!player.IsPlaying) return;
            ResumeProcessing(player.Time);
        };
    }

    private void PrimeProcessesing() {
        Console.WriteLine($"Started");
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
}