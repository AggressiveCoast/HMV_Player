using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.Views;

public partial class PlayVideoView : UserControl {
    public PlayVideoView() {
        InitializeComponent();

        Loaded += (_, _) => { AttachPlayer(); };

        Unloaded += (_, _) => {
            if (DataContext is PlayVideoViewModel vm) {
                if (vm.Player.State == VLCState.Playing) {
                    vm.Player.Pause();
                }
            }
        };
        
        TimeSlider.AddHandler(PointerPressedEvent, TimeSlider_OnPointerPressed, RoutingStrategies.Tunnel);
        TimeSlider.AddHandler(PointerReleasedEvent, TimeSlide_OnPointerReleased, RoutingStrategies.Tunnel);


    }

    private bool _wasPlayingWhenScrubbingStarted = false;
    private bool _isScrubbing;

    private async void AttachPlayer() {
        if (DataContext is PlayVideoViewModel vm) {
            var currentTime = vm.Player.Time;

            VideoViewElement.MediaPlayer = vm.Player;

            if (vm.Player.Media != null) {
                vm.Player.Stop();
                vm.Player.Play();
                vm.Player.Time = currentTime;
                vm.RefreshSliderTextTime();
                TimeSlider.Value = vm.Player.Position;
                // Wait until playback actually starts
                int attempts = 0;
                while (vm.Player.State != VLCState.Playing && attempts < 50) {
                    await Task.Delay(50);
                    attempts++;
                }

                vm.Player.Pause();
            }
        }
    }

    private void TimeSlider_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        _isScrubbing = true;
        if (DataContext is PlayVideoViewModel vm) {
            vm.StartSeeking();
        }
    }

    private void TimeSlide_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        _isScrubbing = false;
        if (DataContext is PlayVideoViewModel vm) {
            vm.EndSeeking();
        }
    }

    private void TimeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e) {
        if (!_isScrubbing) return;
        if (DataContext is PlayVideoViewModel vm) {
            vm.SeekVideo((float)e.NewValue);
        }
    }
}