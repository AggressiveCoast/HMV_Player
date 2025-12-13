using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Controllers;

public class LovenseDeviceController {
    private readonly ILovenseApiService _lovenseApiService;

    public LovenseDeviceController(ILovenseApiService lovenseApiService) {
        _lovenseApiService = lovenseApiService;
    }

    public async Task<bool> PostTestFunction(LovenseToysSettingsFile.LovenseDevice? lovenseToy) {
        if (lovenseToy == null) return false;
        string actionString = "";
        foreach (var lovenseToyFullFunctionName in lovenseToy.RawData.FullFunctionNames) {
            actionString += lovenseToyFullFunctionName + ":1,";
        }

        if (!string.IsNullOrEmpty(actionString)) {
            actionString = actionString[..^1];
        }

        var request = new LovenseFunctionRequest();
        request.Command = "Function";
        request.Action = actionString;
        request.TimeSec = 1;
        request.Toy = lovenseToy.DeviceId;
        request.ApiVer = 1;
        return await _lovenseApiService.PostAction(request);
    }

    /*
     * Vibrate:0 ~ 20
     * Rotate: 0~20
Pump:0~3
Thrusting:0~20
Fingering:0~20
Suction:0~20
Depth: 0~3
Stroke: 0~100
Oscillate:0~20
All:0~20
// Vibrate 9 seconds at 2nd strength
// Rotate toys 9 seconds at 3rd strength
// Pump all toys 9 seconds at 4th strength
// For all toys, it will run 9 seconds then suspend 4 seconds. It will be looped. Total running time is 20 seconds.
{
  "command": "Function",
  "action": "Vibrate:2,Rotate:3,Pump:3",
  "timeSec": 20,
  "loopRunningSec": 9,
  "loopPauseSec": 4,
  "apiVer": 1
}
     */
    public async Task<bool> PostFunction(LovenseToysSettingsFile.LovenseDevice[] lovenseToys, int strength, double seconds) {
        int strengthScaled =(int) (strength / 100f * 20);
        var request = new LovenseFunctionRequest {
            Toy = lovenseToys.Select(device => device.DeviceId).ToArray(),
            StopPrevious = 1,
            Command = "Function",
            Action = $"All:{strengthScaled}",
            TimeSec = seconds,
        };

        int numRetries = 3;
        for (int i = 0; i < numRetries; i++) {
            var success = await _lovenseApiService.PostAction(request);
            if (success) return true;
        }

        return false;
    }

    public async Task<LovenseGetToysResponse> GetToys() {
        return await _lovenseApiService.GetToys();
    }
}