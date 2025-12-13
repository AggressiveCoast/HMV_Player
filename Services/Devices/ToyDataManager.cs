using System.Collections.Generic;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.Services.Devices;

public class ToyDataManager {

    public readonly LovenseToySettingsStorageService LovenseToySettingsStorageService;
    public readonly EdgeToyInterceptorStorageService EdgeToyInterceptorStorageService;
    public ToyDataManager(LovenseToySettingsStorageService lovenseToySettingsStorageService,
        EdgeToyInterceptorStorageService edgeToyInterceptorStorageService) {
        LovenseToySettingsStorageService = lovenseToySettingsStorageService;
        EdgeToyInterceptorStorageService = edgeToyInterceptorStorageService;
    }
}