using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage.Devices;

public class LovenseToySettingsStorageService : BaseSettingsStorageService<LovenseToysSettingsFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.ConfigDir;
    protected override string savePathFileName => "LovenseToySettings";

    public override void LoadObjectPostProcessing(LovenseToysSettingsFile objectLoaded) {
        objectLoaded.PrepareForLoad();
    }

    protected override void SaveObjectPreProcessing(LovenseToysSettingsFile objectToSave) {
        objectToSave.PrepareForSave();
    }
}