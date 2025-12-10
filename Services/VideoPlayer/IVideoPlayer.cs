using System;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace HMV_Player.Services.VideoPlayer;

public interface IVideoPlayer {
    public LibVLC Lib { get; }
    public MediaPlayer Player { get; }

    public long CachedPauseTime { get; set; }
    
    public void Dispose() {
        Player.Dispose();
        Lib.Dispose();
    }

    public void LoadMedia(string filePath);
}