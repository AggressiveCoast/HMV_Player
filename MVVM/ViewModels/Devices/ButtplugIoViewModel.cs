using System;
using System.Linq;
using System.Threading.Tasks;
using Buttplug.Client;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services.Devices.ButtplugIo;

namespace HMV_Player.MVVM.ViewModels.Devices;

public partial class ButtplugIoViewModel : DevicesPageViewModel {
    public override DeviceBrands DeviceBrand => DeviceBrands.ButtplugIo;
    private readonly IButtplugIoClientConnectionService _controllerService;

    public ButtplugIoViewModel(IButtplugIoClientConnectionService controllerService) {
        _controllerService = controllerService;
    }

    public bool IsConnected => _controllerService.IsConnected();
    
    public string[] Devices => _controllerService.GetDevices()?.Select(dev => dev.Name).ToArray();
    
    private ButtplugClientDevice?  _selectedDevice;
    public ButtplugClientDevice? CurrentDevice => _selectedDevice;

    public ButtplugDeviceModel CurrentDeviceModel => new ButtplugDeviceModel() {
        SourceButtplugClientDevice = CurrentDevice
    };

    [RelayCommand]
    public async Task StartConnection() {
        await _controllerService.ConnectToServer();
        _controllerService.GetClient().ServerDisconnect += OnServerDisconnect;
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(Devices));

    }

    private void OnServerDisconnect(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(IsConnected));
    }

    [RelayCommand]
    public async Task StopConnection() {
        _controllerService.GetClient().ServerDisconnect -= OnServerDisconnect;
        await _controllerService.DisconnectFromServer();
        OnPropertyChanged(nameof(IsConnected));
    }
    
    public void OnDeviceChanged(string? deviceName) {
        _selectedDevice = _controllerService.GetDevices()?.FirstOrDefault(dev => dev.Name == deviceName);
    }
}