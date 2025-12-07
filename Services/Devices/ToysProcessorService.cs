using System.Collections.Generic;
using System.Linq;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices;

public class ToysProcessorService {
    public readonly ToyScriptProcessorsStorageService _toyScriptProcessorsStorageService;
    
    public Dictionary<string, ToyScriptProcessor> Devices { get; }

    public ToysProcessorService(ToyScriptProcessorsStorageService toyScriptProcessorsStorageService) {
        _toyScriptProcessorsStorageService = toyScriptProcessorsStorageService;
        Devices = _toyScriptProcessorsStorageService.DataInstance.ToDictionary();
    }

    public void verifyDevices(List<LovenseToy> toysRetrieved) {
        foreach (var toy in toysRetrieved) {
            if (!Devices.ContainsKey(toy.Id)) {
                ToyScriptProcessor toyScriptProcessor = new ToyScriptProcessor(toy.Id, toy.Name);
                Devices.Add(toy.Id, toyScriptProcessor);
            }
        }
    }

    public void PersistData() {
        _toyScriptProcessorsStorageService.DataInstance = ConvertCurrentDevicesToPersistableData();
        _toyScriptProcessorsStorageService.Save();
    }

    private ToyDataPersistable ConvertCurrentDevicesToPersistableData() {
        ToyDataPersistable persistable = new();
        persistable.ToyDatas = Devices.Values.Select((processor => new ToyData(processor))).ToList();
        return persistable;
    }
    
    
}