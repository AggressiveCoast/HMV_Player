using System.Threading.Tasks;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Controllers;

public class LovenseDeviceController {

    private readonly ILovenseApiService _lovenseApiService;
    public LovenseDeviceController(ILovenseApiService lovenseApiService) {
        _lovenseApiService = lovenseApiService;
    }
    public async Task<bool> PostTestFunction(LovenseToy? lovenseToy) {
        if (lovenseToy == null) return false;
        string actionString = "";
        foreach (var lovenseToyFullFunctionName in lovenseToy.FullFunctionNames) {
            actionString += lovenseToyFullFunctionName + ":1,";
        }

        if (!string.IsNullOrEmpty(actionString)) {
            actionString = actionString[..^1];
        }
        
        var request = new LovenseFunctionRequest();
        request.Command = "Function";
        request.Action = actionString;
        request.TimeSec = 1;
        request.Toy = lovenseToy.Id;
        request.ApiVer = 1;
        return await _lovenseApiService.PostAction(request);
    }

    public async Task<LovenseGetToysResponse> GetToys() {
        return await _lovenseApiService.GetToys();
    }
}