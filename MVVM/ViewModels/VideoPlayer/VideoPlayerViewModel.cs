using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services.Storage;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.ViewModels.VideoPlayer;

public partial class VideoPlayerViewModel : ViewModelBase {
    private readonly IVideoPlayer _videoPlayer;
    private readonly UserSettingsStorageService _userSettingsStorageService;
    public MediaPlayer Player => _videoPlayer.Player;

    public bool IsPaused { get; private set; }

    public string SliderTimeText { get; private set; }

    public float SliderTimeValue { get; set; }

    public Action<MediaPlayer> OnMediaLoaded { get; set; }
    public Action<MediaPlayer> OnPausedAction;
    public Action<MediaPlayer> OnResumeAction;

    private bool _wasVideoPlayingBeforeSeeking;

    private bool _isSeeking;

    private int _volume;

    public int Volume {
        get => _volume;
        set {
            _volume = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsVolumeZero));
            OnPropertyChanged(nameof(IsVolume55));
            OnPropertyChanged(nameof(IsVolume100));
            _userSettingsStorageService.DataInstance.DefaultVolume = value;
            Player.Volume = value;
        }
    }

    public bool IsVolumeZero => _volume <= 0;
    public bool IsVolume55 => _volume > 0;
    public bool IsVolume100 => _volume >= 50;

    public long CachedPauseTime {
        get => _videoPlayer.CachedPauseTime;
        set => _videoPlayer.CachedPauseTime = value;
    }


    public VideoPlayerViewModel(IVideoPlayer videoPlayer, UserSettingsStorageService userSettingsStorageService) {
        _videoPlayer = videoPlayer;
        _userSettingsStorageService = userSettingsStorageService;
        IsPaused = true;
        Volume = _userSettingsStorageService.DataInstance.DefaultVolume;
        Player.Playing += PlayerOnPlaying;
        Player.Paused += PlayerOnPaused;
        _videoPlayer.OnMediaLoaded += player => {
            OnMediaLoaded?.Invoke(player);
        };

        Player.SetPause(true);
        SliderTimeText = "00:00:00";
        Player.TimeChanged += PlayerOnTimeChanged;
    }


    private void PlayerOnTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e) {
        if (_isSeeking) return;
        SliderTimeValue = Player.Position;
        OnPropertyChanged(nameof(SliderTimeValue));
        RefreshSliderTextTime(Player.Time);
    }

    private void PlayerOnPlaying(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(IsPaused));
    }

    private void PlayerOnPaused(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(IsPaused));
    }

    [RelayCommand]
    public void PauseToggleVideo() {
        Player.Pause();
        IsPaused = !IsPaused;
        if (IsPaused) {
            OnResumeAction?.Invoke(Player);
        }
        else {
            OnPausedAction?.Invoke(Player);
        }
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
        RefreshSliderTextTime(Player.Time);
    }

    public void RefreshSliderTextTime(long videoTime) {
        var ts = TimeSpan.FromMilliseconds(videoTime);
        SliderTimeText = ts.ToString(@"hh\:mm\:ss");
        OnPropertyChanged(nameof(SliderTimeText));
    }

    public void SaveVolume() {
        _userSettingsStorageService.Save();
    }
}