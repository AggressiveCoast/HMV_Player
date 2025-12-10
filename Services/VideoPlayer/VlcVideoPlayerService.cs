using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace HMV_Player.Services.VideoPlayer;

public class VlcVideoPlayerService : IVideoPlayer{
    
    public LibVLC Lib { get; }
    public MediaPlayer Player { get; }
    public long CachedPauseTime { get; set; }

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

    public void LoadMedia(string filePath) {
        var media = new Media(Lib, filePath,
            FromType.FromPath);
        Player.Media = media;
        Player.Time = 0;
        CachedPauseTime = 0;
    }
}