namespace HMV_Player.Services.Devices.Lovense.Request;

public class LovensePatternV2InitPlayRequest {
    public string command { get; set; } = "PatternV2";
    public string type { get; set; } = "InitPlay";
    public object[] actions { get; set; }
    public int offsetTime { get; set; }
    public int startTime { get; set; }
    public object? toy { get; set; }
    public int stopPrevious { get; set; } // 1 stops previous
    public int apiVer { get; set; } = 1;
}