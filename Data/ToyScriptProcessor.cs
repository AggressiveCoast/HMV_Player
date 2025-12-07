using System;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Data;

public class ToyScriptProcessor {
    
    public bool IsEnabled { get; set; }
    public string ToyId { get; }
    
    public DeviceBrandModelName LovenseToyModelName { get; }
    
    public static DeviceBrandModelName MapDeviceNameToBrandModel(string lovenseToyName) {
        switch (lovenseToyName) {
            case "Gush": return DeviceBrandModelName.LOVENSE_GUSH;
            default: throw new ArgumentOutOfRangeException(lovenseToyName);
        }
    }

    public ToyScriptProcessor(string _toyId, string toyName) {
        ToyId = _toyId;
        LovenseToyModelName = MapDeviceNameToBrandModel(toyName);
    }

    public ToyScriptProcessor(ToyData data) {
        IsEnabled = data.IsEnabled;
        ToyId = data.ToyId;
        LovenseToyModelName = data.DeviceBranchModelName;
    }
    
}