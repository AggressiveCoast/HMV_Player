using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage.Devices;

public class TheHandySettingsStorageService : BaseSettingsStorageService<TheHandySettingsFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.ConfigDir;
    protected override string savePathFileName => "TheHandySettings";
}