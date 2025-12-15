using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense.Request;

public class LovensePatternV2PlayRequest {
    public string command { get; set; } = "PatternV2";
    public string type { get; set; } = "Play";
    public object toy { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int startTime { get; set; } = 0;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? offsetTime { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? timeMs { get; set; }
    public int apiVer { get; set; } = 1;
}