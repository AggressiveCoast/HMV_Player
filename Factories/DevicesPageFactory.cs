using System;
using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels;
using HMV_Player.MVVM.ViewModels.Devices;

namespace HMV_Player.Factories;

public class DevicesPageFactory(Func<DeviceBrands, DevicesPageViewModel> factory) {
    
    public DevicesPageViewModel GetDevicesPageViewModel(DeviceBrands brandName) => factory.Invoke(brandName);

}