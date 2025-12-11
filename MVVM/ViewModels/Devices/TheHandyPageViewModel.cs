using HMV_Player.Data;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.MVVM.ViewModels.Devices;

public class TheHandyPageViewModel : DevicesPageViewModel {
    public override DeviceBrands DeviceBrand => DeviceBrands.TheHandy;
    
    private readonly TheHandySettingsStorageService _theHandySettingsStorageService;
    
    private string _connectionKey;

    public TheHandyPageViewModel(TheHandySettingsStorageService theHandySettingsStorageService) {
        _theHandySettingsStorageService = theHandySettingsStorageService;
        ConnectionKey = _theHandySettingsStorageService.DataInstance.ConnectionKey;
    }

    public string ConnectionKey {
        get {
            return _connectionKey;
        }
        set {
            _connectionKey = value;
            OnPropertyChanged(nameof(ConnectionKey));
            _theHandySettingsStorageService.DataInstance.ConnectionKey = value;
            _theHandySettingsStorageService.Save();
        }
    }
}