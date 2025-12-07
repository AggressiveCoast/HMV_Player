using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.Factories;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.Devices;

namespace HMV_Player.MVVM.ViewModels;

public partial class DevicesViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.Devices;

    private DevicesPageFactory _factory;

    public DevicesViewModel(DevicesPageFactory factory) {
        _factory = factory;
        GoToLovensePage();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLovensePageActive))]
    [NotifyPropertyChangedFor(nameof(IsNogasmPageActive))]
    private DevicesPageViewModel _currentDevicesPage;
    
    public bool IsLovensePageActive => CurrentDevicesPage.DeviceBrand == DeviceBrands.Lovense;
    
    public bool IsNogasmPageActive => CurrentDevicesPage.DeviceBrand == DeviceBrands.Nogasm;
    
    
    [RelayCommand]
    private void GoToLovensePage() {
        CurrentDevicesPage = _factory.GetDevicesPageViewModel(DeviceBrands.Lovense);
    }

    [RelayCommand]
    private void GoToNogasmPage() {
        CurrentDevicesPage = _factory.GetDevicesPageViewModel(DeviceBrands.Nogasm);
    }
}