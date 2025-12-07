using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels.Base;

namespace HMV_Player.MVVM.ViewModels.Devices;

public abstract class DevicesPageViewModel : ViewModelBase {
    public abstract DeviceBrands DeviceBrand { get; }
}