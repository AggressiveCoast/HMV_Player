using System;
using HMV_Player.Data;
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

    public bool PressureThresholdReached(FunScriptChannel channel) {
        if (channel == FunScriptChannel.Channel1 &&
            !_edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel1) {
            return false;
        }
        if (channel == FunScriptChannel.Channel2 &&
            !_edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel2) {
            return false;
        }
        if (channel == FunScriptChannel.Channel3 &&
            !_edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel3) {
            return false;
        }
        if (!_nogasmAnalyzerService.IsTrackingPort) return false;
        return _nogasmAnalyzerService.CurrentPressure > _edgeToyInterceptorStorageService.DataInstance.nogasmData
            .OtherDevicePausePressureThreshold;
    }

    public void StopInterceptorTracking() {
        _nogasmAnalyzerService.StopTrackingPort();
    }
}