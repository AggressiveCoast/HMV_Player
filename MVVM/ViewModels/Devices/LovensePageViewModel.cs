using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Controls;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Factories;
using HMV_Player.Services;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.MVVM.ViewModels.Devices;

public partial class LovensePageViewModel : DevicesPageViewModel {
    public override DeviceBrands DeviceBrand => DeviceBrands.Lovense;

    private readonly LovenseDeviceController _lovenseDeviceController;
    private readonly LovenseToySettingsStorageService _lovenseToySettingsStorageService;
    private readonly NotificationContainerViewModel _notificationContainerViewModel;
    public LovensePageViewModel(LovenseDeviceController lovenseApiService,
        LovenseToySettingsStorageService lovenseToySettingsStorageService, NotificationContainerViewModel notificationContainerViewModel) {
        _lovenseDeviceController = lovenseApiService;
        _lovenseToySettingsStorageService = lovenseToySettingsStorageService;
        _notificationContainerViewModel = notificationContainerViewModel;
    }

    public List<LovenseToy> ToyDataList {
        get {
            return _lovenseToySettingsStorageService.DataInstance.DevicesDict.Values.Select(device => device.RawData)
                .ToList();
        }
    }

    public bool IsToysListEmpty => ToyDataList.Count == 0;

    public string NoAvailableToysMessage { get; set; }

    public int BatteryAmount => CurrentSelectedLovenseToy == null ? 0 : CurrentSelectedLovenseToy.Battery;

    [ObservableProperty] private bool _isToyScriptEnabled;

    [ObservableProperty] private LovenseToy? _currentSelectedLovenseToy;

    [RelayCommand]
    public async Task RefreshLovenseToysList() {
        try {
            var results = await _lovenseDeviceController.GetToys();
            if (results.Code == 402) {
                NoAvailableToysMessage = "No Toys Connected";
            }
            else if (results.Code != 200) {
                NoAvailableToysMessage = "Unable to connect to local Lovense Remote App";
            }

            // add any toys to storage if they don't exist
            if (results.Data.Toys.Count > 0) {
                foreach (var keyValuePair in results.Data.Toys) {
                    if (_lovenseToySettingsStorageService.DataInstance.DevicesDict.TryGetValue(keyValuePair.Key,
                            out var lovenseDevice)) {
                        lovenseDevice.RawData = keyValuePair.Value;
                    }
                    else {
                        _lovenseToySettingsStorageService.DataInstance.DevicesDict.Add(keyValuePair.Key,
                            new LovenseToysSettingsFile.LovenseDevice() {
                                DeviceId = keyValuePair.Key,
                                Enabled = true,
                                RawData = keyValuePair.Value
                            });
                    }
                }
            }
            
            // if can't find lovense toy connect, make sure to set the battery to 0 so it isn't misleading
            foreach (var keyValuePair in _lovenseToySettingsStorageService.DataInstance.DevicesDict) {
                if (!results.Data.Toys.ContainsKey(keyValuePair.Key)) {
                    keyValuePair.Value.RawData.Battery = 0;
                }
            }
            

            if (ToyDataList.Count > 0) {
                CurrentSelectedLovenseToy = ToyDataList.First();
            }
        }
        catch (Exception e) {
            NoAvailableToysMessage = "Unable to connect to local Lovense Remote App";
            CurrentSelectedLovenseToy = null;
        }

        OnPropertyChanged(nameof(ToyDataList));
        OnPropertyChanged(nameof(IsToysListEmpty));
        OnPropertyChanged(nameof(NoAvailableToysMessage));
        OnPropertyChanged(nameof(BatteryAmount));
    }

    [RelayCommand]
    public async Task TestDevice() {
        bool success = await _lovenseDeviceController.PostTestFunction(CurrentSelectedLovenseToy);
        if (!success) {
            string toy = CurrentSelectedLovenseToy == null ? string.Empty : CurrentSelectedLovenseToy.Name;
            _notificationContainerViewModel.ShowNotification("Device Test Failed",toy , NotificationType.Warning);
        }
    }

    partial void OnCurrentSelectedLovenseToyChanged(LovenseToy toy) {
        if (toy == null) return;
        var lovenseDevice = GetCurrentLovenseDevice();

        loadDataIntoXaml(lovenseDevice);
    }

    partial void OnIsToyScriptEnabledChanged(bool value) {
        GetCurrentLovenseDevice().Enabled = value;
    }

    private LovenseToysSettingsFile.LovenseDevice GetCurrentLovenseDevice() {
        return _lovenseToySettingsStorageService.DataInstance.DevicesDict[CurrentSelectedLovenseToy.Id];
    }

    private void loadDataIntoXaml(LovenseToysSettingsFile.LovenseDevice currentLovenseDevice) {
        IsToyScriptEnabled = currentLovenseDevice.Enabled;
    }

    public void OnPageLoaded() {
        OnPropertyChanged(nameof(ToyDataList));
        OnPropertyChanged(nameof(ToyDataList));
        OnPropertyChanged(nameof(IsToysListEmpty));
        if (ToyDataList.Count > 0) {
            CurrentSelectedLovenseToy = ToyDataList.First();
        }

        _ = RefreshLovenseToysList();
    }

    public void OnPageUnloaded() {
        _lovenseToySettingsStorageService.Save();
    }
}