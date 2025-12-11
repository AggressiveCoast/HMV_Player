using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Storage.Devices;

public class EdgeToyInterceptorStorageService : BaseSettingsStorageService<EdgeToyInterceptorStorageFile> {
    protected override string baseFolderPath => HMVPlayerAppPaths.ConfigDir;
    protected override string savePathFileName => "EdgeToyInterceptorSettings";
    
    public const int MAX_PRESSURE_THRESHOLD = 10000;
}