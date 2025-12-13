using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Funscript;

public class FunscriptPlayer {

    private FunScriptChannel _channel;
    private FunscriptWrapper _funscriptWrapper;
    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;
    private readonly LovenseDeviceController _lovenseController;
    private readonly ToyDataManager _toyDataManager;
    private int _lastFoundIndex = 0;

    private LovenseToysSettingsFile.LovenseDevice[] _applicableLovenseToys;
    public FunscriptPlayer(FunScriptChannel channel, FunscriptFile funscriptFile, LovenseDeviceController lovenseController, EdgeToyInterceptorService edgeToyInterceptorService, ToyDataManager toyDataManager) {
        _channel = channel;
        _funscriptWrapper = new(funscriptFile);
        _lovenseController = lovenseController;
        _toyDataManager = toyDataManager;
        _edgeToyInterceptorService = edgeToyInterceptorService;
        _applicableLovenseToys = _toyDataManager.LovenseToySettingsStorageService.DataInstance.DevicesDict.Values
            .Where(dev => dev.ApplicableChannel == channel && dev.Enabled).ToArray();
    }

    public void ProcessFunscript(long playerTime, CancellationToken cancellationToken) {
        int currentIndex = _funscriptWrapper.GetCurrentIndexFromTime(playerTime);
        if (currentIndex == _lastFoundIndex || currentIndex == _funscriptWrapper.SourceFunscriptFile.actions.Length) return;
        _lastFoundIndex = currentIndex;
        FunscriptFile.Action action = _funscriptWrapper.GetActionAtIndex(currentIndex)!;
        if (currentIndex + 1 >= _funscriptWrapper.SourceFunscriptFile.actions.Length) {
            _ = HandleEndReached();
            return;
        }

        if (_edgeToyInterceptorService.PressureThresholdReached()) {
            _ = HandleEdgeInterceptor();
            return;
        }
        
        FunscriptFile.Action nextAction = _funscriptWrapper.GetActionAtIndex(currentIndex + 1)!;
        
        _ = HandleLovense(action, nextAction);
    }

    private async Task HandleEndReached() {
        await _lovenseController.PostFunction(_applicableLovenseToys, 0, 1);
    }

    private async Task HandleEdgeInterceptor() {
        await _lovenseController.PostFunction(_applicableLovenseToys, 0, _toyDataManager.EdgeToyInterceptorStorageService.DataInstance.nogasmData.NumberOfSecondsToStopBlocking);
    }

    private async Task HandleLovense(FunscriptFile.Action currentAction, FunscriptFile.Action nextAction) {
        // convert funscript data to vibrations
        int strength = 0;
        int positionDif = nextAction.pos - currentAction.pos;
        double seconds = nextAction.at - currentAction.at;
        await _lovenseController.PostFunction(_applicableLovenseToys, strength, seconds);
    }
    
    
}