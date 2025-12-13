using System.Collections.Generic;
using System.Text.Json.Serialization;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Data.Persistable;

public class LovenseToysSettingsFile {

    [JsonInclude]
    private List<LovenseDevice> Devices { get; set; } = new();
    public Dictionary<string, LovenseDevice> DevicesDict = new();

    public void PrepareForSave() {
        Devices = new();
        foreach (var keyValuePair in DevicesDict) {
            Devices.Add(keyValuePair.Value);
        }
    }

    public void PrepareForLoad() {
        DevicesDict = new();
        foreach (var objectLoadedDevice in Devices) {
            DevicesDict.Add(objectLoadedDevice.DeviceId, objectLoadedDevice);
        }
    }
    public class LovenseDevice {
        public bool Enabled { get; set; } = false;
        public string DeviceId { get; set; }
        public LovenseToy RawData { get; set; }

        public FunScriptChannel ApplicableChannel { get; set; } = FunScriptChannel.Channel1;
    }
}