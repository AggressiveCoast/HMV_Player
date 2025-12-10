using System.Collections.Generic;
using System.Linq;

namespace HMV_Player.Data.Persistable;

public class VideoStorageDataFile {

    public string? BaseLocation { get; set; } = "";

    public HashSet<VideoFileData> VideoFileDatas { get; set; } = new();

    public VideoFileData GetFileData(string videoPath) {
        return VideoFileDatas.FirstOrDefault((data => data.FullPath == videoPath));
    }
}