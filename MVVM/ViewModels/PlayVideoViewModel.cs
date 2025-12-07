using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.ViewModels;

public partial class PlayVideoViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.PlayVideo;

    public MediaPlayer Player => _playerService.Player;
    private readonly IVideoPlayer _playerService;

    public bool IsPaused { get; private set; }

    public string SliderTimeText { get; private set; }
    
    public float SliderTimeValue { get; set; }

    private bool _wasVideoPlayingBeforeSeeking;

    private bool _isSeeking;

    public PlayVideoViewModel(IVideoPlayer videoPlayer) {
        _playerService = videoPlayer;

        IsPaused = true;

        Player.Playing += PlayerOnPlaying;
        Player.Paused += PlayerOnPaused;
        Player.Stopped += PlayerOnStopped;

        Player.SetPause(true);
        SliderTimeText = "00:00:00";
        Player.TimeChanged += PlayerOnTimeChanged;
    }



    private void PlayerOnTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e) {
        if (_isSeeking) return;
        SliderTimeValue = Player.Position;
        OnPropertyChanged(nameof(SliderTimeValue));
        RefreshSliderTextTime();
    }

    private void PlayerOnPlaying(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(IsPaused));
        _playerService.OnPlaying();
    }

    private void PlayerOnPaused(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(IsPaused));
        _playerService.OnPaused();
    }
    
    private void PlayerOnStopped(object? sender, EventArgs e) {
        _playerService.OnStopped();
    }

    [RelayCommand]
    public void PauseToggleVideo() {
        Player.Pause();
        IsPaused = !IsPaused;
        OnPropertyChanged(nameof(IsPaused));
    }

    [RelayCommand]
    public void ToggleFullScreen() {
        Player.ToggleFullscreen();
    }

    [RelayCommand]
    public async void LoadMedia() {
        var media = new Media(_playerService.Lib, @"D:\Stuff\Clips\Evil Baka\013_Kiriko\EVILBAKA 4K 013-1.mp4",
            FromType.FromPath);
        Player.Media = media;
        Player.Play();

        // Wait for media to be parsed and playback to start
        while (Player.Length <= 0 && Player.State != VLCState.Playing) {
            await Task.Delay(50);
        }

        // Seek to first frame (position 0.0 = start)
        Player.Position = 0.0f;
        SliderTimeValue  = 0.0f;
        OnPropertyChanged(nameof(SliderTimeValue));

        // Small delay to ensure frame is rendered
        await Task.Delay(50);

        Player.Pause();
        IsPaused = true;
        OnPropertyChanged(nameof(IsPaused));
    }

    public void StartSeeking() {
        _isSeeking = true;
        _wasVideoPlayingBeforeSeeking = Player.State == VLCState.Playing;
        Player.SetPause(true);
        IsPaused = true;
        OnPropertyChanged(nameof(IsPaused));
    }

    public void EndSeeking() {
        _isSeeking = false;
        if (_wasVideoPlayingBeforeSeeking) {
            Player.SetPause(false);
            IsPaused = false;
            OnPropertyChanged(nameof(IsPaused));
        }
    }

    public void SeekVideo(float normalizedTime) {
        Player.Position = normalizedTime;
        RefreshSliderTextTime();
    }

    public void RefreshSliderTextTime() {
        var ts = TimeSpan.FromMilliseconds(Player.Time);
        SliderTimeText = ts.ToString(@"hh\:mm\:ss");
        OnPropertyChanged(nameof(SliderTimeText));
    }
    
    
    
    
}