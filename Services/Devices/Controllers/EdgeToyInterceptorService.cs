using System;
using HMV_Player.Services.Funscript;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.Services.Devices;

public class EdgeToyInterceptorService {
    private readonly NogasmAnalyzerService _nogasmAnalyzerService;
    private readonly EdgeToyInterceptorStorageService _edgeToyInterceptorStorageService;
    
    public EdgeToyInterceptorService(EdgeToyInterceptorStorageService edgeToyInterceptorStorageService, NogasmAnalyzerService nogasmAnalyzerService) {
        _nogasmAnalyzerService = nogasmAnalyzerService;
        _edgeToyInterceptorStorageService = edgeToyInterceptorStorageService;
    }

    public void StartInterceptorTracking() {
        if (_edgeToyInterceptorStorageService.DataInstance.nogasmData.IsNogasmTrackingEnabled) {
            string nogasmPort = _edgeToyInterceptorStorageService.DataInstance.nogasmData.NogasmPort;
            _nogasmAnalyzerService.StartTrackingPort(nogasmPort);
        }

    }

    public void StopInterceptorTracking() {
        _nogasmAnalyzerService.StopTrackingPort();
    }

    public Action OnInterceptorThresholdCrossed;

}