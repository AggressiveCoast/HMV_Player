using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HMV_Player.Data.Persistable;

public class VideoStorageDataFile {

    public string? BaseLocation { get; set; } = "";

    [JsonInclude]
    private List<VideoFileData> VideoFileDatas { get; set; } = new();

    public Dictionary<string, VideoFileData?> VideoFileDatasDict;

    public void PersistDictionaryToList() {
        VideoFileDatas = VideoFileDatasDict.Values.ToList();
    }

    public void LoadDictionaryFromList() {
        VideoFileDatasDict = new();
        foreach (var videoFileData in VideoFileDatas) {
            VideoFileDatasDict.Add(videoFileData.FullPath, videoFileData);
        }
    }
}