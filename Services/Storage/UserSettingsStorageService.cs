using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage;

public class UserSettingsStorageService : BaseSettingsStorageService<UserSettingsStorageFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.ConfigDir;
    protected override string savePathFileName => "UserSettings";
}