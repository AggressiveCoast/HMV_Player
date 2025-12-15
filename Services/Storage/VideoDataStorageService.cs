using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage;

public class VideoDataStorageService : BaseSettingsStorageService<VideoStorageDataFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.CacheDir;
    protected override string savePathFileName => "VideoData";

    protected override void SaveObjectPreProcessing(VideoStorageDataFile objectToSave) {
        base.SaveObjectPreProcessing(objectToSave);
        objectToSave.PersistDictionaryToList();
    }

    public override void LoadObjectPostProcessing(VideoStorageDataFile objectLoaded) {
        base.LoadObjectPostProcessing(objectLoaded);
        objectLoaded.LoadDictionaryFromList();
    }
}