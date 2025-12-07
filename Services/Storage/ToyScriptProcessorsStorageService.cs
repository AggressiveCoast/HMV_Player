using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Storage;

namespace HMV_Player.Services;

public class ToyScriptProcessorsStorageService : BaseSettingsStorageService<ToyDataPersistable> {
    protected override string baseFolderPath => HMVPlayerAppPaths.ConfigDir;
    protected override string savePathFileName => "ToysEnabledData";

}