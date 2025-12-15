namespace HMV_Player.Services.Devices.Lovense.Request;

public class LovensePatternV2StopRequest {
    public string command { get; set; } = "PatternV2";
    public string type { get; set; } = "Stop";
    public object toy { get; set; }
    public string apiVer { get; set; } = "1";
}