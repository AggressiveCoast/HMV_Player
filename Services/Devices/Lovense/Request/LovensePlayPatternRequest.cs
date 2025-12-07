using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense;

public class LovensePlayPatternRequest {
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    // Optional: 0–7,200,000 ms
    [JsonPropertyName("startTime")]
    public int? StartTime { get; set; }

    // Optional: 0–15,000 ms
    [JsonPropertyName("offsetTime")]
    public int? OffsetTime { get; set; }

    // Optional: must be > 100 to be accepted
    [JsonPropertyName("timeMs")]
    public double? TimeMs { get; set; }

    // Optional: single ID or array of IDs
    // If null → apply to all toys
    [JsonPropertyName("toy")]
    public object? Toy { get; set; }

    [JsonPropertyName("apiVer")]
    public int ApiVer { get; set; } = 1;
}