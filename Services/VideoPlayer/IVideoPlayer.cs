using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace HMV_Player.Services.VideoPlayer;

public interface IVideoPlayer {
    public LibVLC Lib { get; }
    public MediaPlayer Player { get; }

    public void OnPlaying() {
        OnPlayingAction?.Invoke(Player);
    }

    public void OnPaused() {
        OnPausedAction?.Invoke(Player);
    }

    public void OnStopped() {
        OnStoppedAction?.Invoke(Player);
    }

    public Action<MediaPlayer> OnPlayingAction { get; }
    public Action<MediaPlayer> OnPausedAction { get; }
    public Action<MediaPlayer> OnStoppedAction { get; }

    public void Dispose() {
        Player.Dispose();
        Lib.Dispose();
    }
}