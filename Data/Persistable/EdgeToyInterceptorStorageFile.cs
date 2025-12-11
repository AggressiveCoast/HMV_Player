namespace HMV_Player.Data.Persistable;

public class EdgeToyInterceptorStorageFile {

    public NogasmData nogasmData { get; set; } = new();
    public class NogasmData {
        public bool IsNogasmTrackingEnabled { get; set; }
        public string NogasmPort { get; set; }
        public int OtherDevicePausePressureThreshold { get; set; } = 1000;
        public bool InterceptChannel1 { get; set; } = true;
        public bool InterceptChannel2 { get; set; }
        public bool InterceptChannel3 { get; set; }

    }
}