using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense;

public class LovenseSetupPatternRequest {
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("actions")]
    public List<ActionEntry> Actions { get; set; } = new();
    
    [JsonPropertyName("apiVer")]
    public int ApiVer { get; set; } = 1;
}

public class ActionEntry {
    public int Ts { get; set; }
    public int Pos { get; set; } 
}