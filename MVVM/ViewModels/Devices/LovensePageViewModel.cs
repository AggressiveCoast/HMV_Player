using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        LovenseToySettingsStorageService lovenseToySettingsStorageService,
        NotificationContainerViewModel notificationContainerViewModel) {
        _lovenseDeviceController = lovenseApiService;
        _lovenseToySettingsStorageService = lovenseToySettingsStorageService;
        _notificationContainerViewModel = notificationContainerViewModel;
    }

    public ObservableCollection<LovenseToysSettingsFile.LovenseDevice> ToyDataList { get; set; } = new();


    public bool IsToysListEmpty => ToyDataList.Count == 0;

    public string NoAvailableToysMessage { get; set; }

    public int BatteryAmount => CurrentSelectedLovenseToy == null ? 0 : CurrentSelectedLovenseToy.RawData.Battery;

    public string DeviceIdText {
        get {
            if (CurrentSelectedLovenseToy == null) return string.Empty;
            return "Device ID: " + CurrentSelectedLovenseToy.RawData.Id;
        }
    }

    public string DeviceBatteryText {
        get {
            if (CurrentSelectedLovenseToy == null) return string.Empty;
            return "Battery: " + CurrentSelectedLovenseToy.RawData.Battery + "%";
        }
    }

    public string DeviceNickNameText {
        get {
            if (CurrentSelectedLovenseToy == null) return string.Empty;

            return "Nickname: " + CurrentSelectedLovenseToy.RawData.NickName;
        }
    }

    public string DeviceFunctionNamesText {
        get {
            if (CurrentSelectedLovenseToy == null) return string.Empty;

            string functions = "";
            foreach (var rawDataFullFunctionName in CurrentSelectedLovenseToy.RawData.FullFunctionNames) {
                functions += rawDataFullFunctionName + ",";
            }

            if (!string.IsNullOrEmpty(functions)) {
                functions = new string(functions.ToCharArray()[..^1]);
            }
            return "Functions: " + functions;
        }
    }

    public List<string> ComboBoxPossibleOptions { get; set; } = new() {
        "Channel 1",
        "Channel 2",
        "Channel 3"
    };

    public string SelectedChannelComboBoxValue {
        get;
        set;
    }

    [ObservableProperty] private bool _isToyScriptEnabled;

    [ObservableProperty] private LovenseToysSettingsFile.LovenseDevice? _currentSelectedLovenseToy;

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

            ToyDataList.Clear();
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

                    ToyDataList.Add(_lovenseToySettingsStorageService.DataInstance.DevicesDict[keyValuePair.Key]);
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
        loadDataIntoXaml(CurrentSelectedLovenseToy);
        refreshToyProperties();
    }

    [RelayCommand]
    public async Task TestDevice() {
        bool success = await _lovenseDeviceController.PostTestFunction(CurrentSelectedLovenseToy);
        if (!success) {
            string toy = CurrentSelectedLovenseToy == null ? string.Empty : CurrentSelectedLovenseToy.RawData.Name;
            _notificationContainerViewModel.ShowNotification("Device Test Failed", toy, NotificationType.Warning);
        }
    }

    partial void OnCurrentSelectedLovenseToyChanged(LovenseToysSettingsFile.LovenseDevice? toy) {
        if (toy == null) return;
        var lovenseDevice = GetCurrentLovenseDevice();

        loadDataIntoXaml(lovenseDevice);
        refreshToyProperties();
    }

    partial void OnIsToyScriptEnabledChanged(bool value) {
        GetCurrentLovenseDevice().Enabled = value;
    }

    private LovenseToysSettingsFile.LovenseDevice GetCurrentLovenseDevice() {
        return _lovenseToySettingsStorageService.DataInstance.DevicesDict[CurrentSelectedLovenseToy.RawData.Id];
    }

    private void loadDataIntoXaml(LovenseToysSettingsFile.LovenseDevice? currentLovenseDevice) {
        if (currentLovenseDevice == null) return;
        IsToyScriptEnabled = currentLovenseDevice.Enabled;

        // combo box text
        try {
            SelectedChannelComboBoxValue = ComboBoxPossibleOptions[(int)currentLovenseDevice.ApplicableChannel];
        }
        catch (IndexOutOfRangeException e) {
            NotificationService.ShowNotification("Invalid Channel Stored.",
                "Lovense Device has an invalid channel stored, resetting to 1.", NotificationType.Error);
            currentLovenseDevice.ApplicableChannel = FunScriptChannel.Channel1;
            SelectedChannelComboBoxValue =  "Channel 1";
        }
    }

    public void OnPageLoaded() {
        _ = RefreshLovenseToysList();
    }

    public void OnPageUnloaded() {
        _lovenseToySettingsStorageService.Save();
    }

    public void ScriptChannelComboBoxUpdated(int channel) {
        CurrentSelectedLovenseToy.ApplicableChannel = (FunScriptChannel)channel;
    }

    private void refreshToyProperties() {
        OnPropertyChanged(nameof(CurrentSelectedLovenseToy));
        if (CurrentSelectedLovenseToy != null) {
            OnPropertyChanged(nameof(SelectedChannelComboBoxValue));
            OnPropertyChanged(nameof(DeviceIdText));
            OnPropertyChanged(nameof(DeviceBatteryText));
            OnPropertyChanged(nameof(DeviceFunctionNamesText));
            OnPropertyChanged(nameof(DeviceNickNameText));

        }
    }
}