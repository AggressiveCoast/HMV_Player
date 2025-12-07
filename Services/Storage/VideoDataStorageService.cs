using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage;

public class VideoDataStorageService : BaseSettingsStorageService<VideoStorageDataFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.CacheDir;
    protected override string savePathFileName => "VideoData";
}