using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense.Response;
using HMV_Player.Services.Funscript.Lovense;

namespace HMV_Player.Services.Funscript;

public class FunscriptPlayer {
    private FunScriptChannel _channel;
    private FunscriptWrapper _funscriptWrapper;
    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;
    private readonly LovenseDeviceController _lovenseController;
    private readonly ToyDataManager _toyDataManager;
    private int _lastFoundIndex = -1;
    
    private LovenseFunscriptPlayer _lovenseFunscriptPlayer;


    public FunscriptPlayer(FunScriptChannel channel, FunscriptFile funscriptFile,
        LovenseDeviceController lovenseController, EdgeToyInterceptorService edgeToyInterceptorService,
        ToyDataManager toyDataManager) {
        _channel = channel;
        _funscriptWrapper = new(funscriptFile);
        _lovenseController = lovenseController;
        _toyDataManager = toyDataManager;
        _edgeToyInterceptorService = edgeToyInterceptorService;
        _lovenseFunscriptPlayer = new LovenseFunscriptPlayer(channel, toyDataManager, _funscriptWrapper, lovenseController, edgeToyInterceptorService);
    }



    public void ProcessFunscript(long playerTime, bool isPlaying, CancellationToken cancellationToken) {
        _lovenseFunscriptPlayer.ProcessFunScript(playerTime, isPlaying);
    }

    public void PauseDevices() {
        _lovenseFunscriptPlayer.PauseDevices();
    }

    
}