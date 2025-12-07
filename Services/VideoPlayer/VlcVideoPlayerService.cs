using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace HMV_Player.Services.VideoPlayer;

public class VlcVideoPlayerService : IVideoPlayer{
    
    public LibVLC Lib { get; }
    public MediaPlayer Player { get; }
    public Action<MediaPlayer> OnPlayingAction { get; }
    public Action<MediaPlayer> OnPausedAction { get; }
    public Action<MediaPlayer> OnStoppedAction { get; }

    public VlcVideoPlayerService()
    {
        Core.Initialize();
        Lib = new LibVLC();
        Player = new MediaPlayer(Lib);
    }

    public void Dispose()
    {
        Player.Dispose();
        Lib.Dispose();
    }
}