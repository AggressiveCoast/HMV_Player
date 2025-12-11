using HMV_Player.Services.Devices.TheHandy;

namespace HMV_Player.Services.Devices.Controllers;

public class TheHandyDeviceController {

    private readonly TheHandyApiService _theHandyApiService;
    
    public TheHandyDeviceController(TheHandyApiService theHandyApiService) {
        _theHandyApiService = theHandyApiService;
    }
}