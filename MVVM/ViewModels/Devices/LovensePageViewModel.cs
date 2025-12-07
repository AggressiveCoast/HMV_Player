using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Factories;
using HMV_Player.Services;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Lovense.API;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.MVVM.ViewModels.Devices;

public partial class LovensePageViewModel : DevicesPageViewModel {
    public override DeviceBrands DeviceBrand => DeviceBrands.Lovense;

    private readonly ILovenseApiService _lovenseApiService;
    private readonly ToysProcessorService _toyProcessor;

    public LovensePageViewModel(ILovenseApiService lovenseApiService, ToysProcessorService toyScriptProcessor) {
        _lovenseApiService = lovenseApiService;
        _toyProcessor = toyScriptProcessor;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsToysListEmpty))]
    [NotifyPropertyChangedFor(nameof(NoAvailableToysMessage))]
    private List<LovenseToy> _toyDataList = new();

    public bool IsToysListEmpty => ToyDataList.Count == 0;

    public string NoAvailableToysMessage { get; set; }

    [ObservableProperty] private bool _isToyScriptEnabled;

    [ObservableProperty] private LovenseToy _currentSelectedLovenseToy;

    [RelayCommand]
    public async Task RefreshLovenseToysList() {
        try {
            var results = await _lovenseApiService.GetToys();
            if (results.Code == 402) {
                NoAvailableToysMessage = "No Toys Connected";
            }
            else if (results.Code != 200) {
                NoAvailableToysMessage = "Unable to connect to local Lovense Remote App";
            }

            ToyDataList = results.Data.Toys.Values.ToList();
            _toyProcessor.verifyDevices(ToyDataList);
            if (ToyDataList.Count > 0) {
                CurrentSelectedLovenseToy = ToyDataList[0];
            }
        }
        catch (Exception e) {
            NoAvailableToysMessage = "Unable to connect to local Lovense Remote App";
            ToyDataList = new();
            CurrentSelectedLovenseToy = null;
        }
    }

    partial void OnCurrentSelectedLovenseToyChanged(LovenseToy toy) {
        if (toy == null) return;
        var processor = GetCurrentToyScriptProcessor();

        loadDataIntoXaml(processor);
    }

    partial void OnIsToyScriptEnabledChanged(bool value) {
        GetCurrentToyScriptProcessor().IsEnabled = value;
        _toyProcessor.Devices[GetCurrentToyScriptProcessor().ToyId].IsEnabled = value;
    }

    private ToyScriptProcessor GetCurrentToyScriptProcessor() {
        DeviceBrandModelName toyModelName = ToyScriptProcessor.MapDeviceNameToBrandModel(CurrentSelectedLovenseToy.Name);
        return _toyProcessor.Devices[CurrentSelectedLovenseToy.Id];
    }

    private void loadDataIntoXaml(ToyScriptProcessor currentScriptProcessor) {
        IsToyScriptEnabled = currentScriptProcessor.IsEnabled;
    }

    public void OnPageLoaded() {
        RefreshLovenseToysList();
    }

    public void OnPageUnloaded() {
        _toyProcessor.PersistData();
    }

    
}