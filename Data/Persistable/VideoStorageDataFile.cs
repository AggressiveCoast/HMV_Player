using System.Collections.Generic;

namespace HMV_Player.Data.Persistable;

public class VideoStorageDataFile {

    public string? BaseLocation { get; set; } = "";

    public HashSet<VideoFileData> VideoFileDatas { get; set; } = new();
}