using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.Views.VideoPlayer;

public partial class VideoPlayerView : UserControl {
    
    public VideoPlayerView() {
        InitializeComponent();

        Loaded += (_, _) => { AttachPlayer(); };

        Unloaded += (_, _) => {
            if (DataContext is VideoPlayerViewModel vm) {
                if (vm.Player.State == VLCState.Playing) {
                    vm.Player.Pause();
                }
                vm.CachedPauseTime = vm.Player.Time;
                vm.SaveVolume();
                vm.SetVideoPlayerState(VideoPlayerViewModel.VideoPlayerState.Paused);
            }
        };

        

        TimeSlider.AddHandler(PointerPressedEvent, TimeSlider_OnPointerPressed, RoutingStrategies.Tunnel);
        TimeSlider.AddHandler(PointerReleasedEvent, TimeSlide_OnPointerReleased, RoutingStrategies.Tunnel);
        TimeSlider.ValueChanged += TimeSlider_OnValueChanged;
        
    }

    private bool _wasPlayingWhenScrubbingStarted = false;
    private bool _isScrubbing;

    private async void AttachPlayer() {
        if (DataContext is VideoPlayerViewModel vm) {

            VideoViewElement.MediaPlayer = vm.Player;

            if (vm.Player.Media != null) {
                vm.Player.Stop();
                vm.Player.Play();
                vm.Player.Time = vm.CachedPauseTime;
                // Wait until playback actually starts
                int attempts = 0;
                while (vm.Player.State != VLCState.Playing && attempts < 50) {
                    await Task.Delay(50);
                    attempts++;
                }
                vm.Player.Pause();
                vm.RefreshSliderTextTime(vm.CachedPauseTime);
                TimeSlider.Value = (double) vm.CachedPauseTime / vm.Player.Length;
                vm.SliderTimeValue = (float)vm.CachedPauseTime / vm.Player.Length;
            }
        }
    }

    private void TimeSlider_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        _isScrubbing = true;
        if (DataContext is VideoPlayerViewModel vm) {
            vm.StartSeeking();
        }
    }

    private void TimeSlide_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        _isScrubbing = false;
        if (DataContext is VideoPlayerViewModel vm) {
            vm.EndSeeking();
        }
    }

    private void TimeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e) {
        if (!_isScrubbing) return;
        if (DataContext is VideoPlayerViewModel vm) {
            vm.SeekVideo((float)e.NewValue);
        }
    }
}