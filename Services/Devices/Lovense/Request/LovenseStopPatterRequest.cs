using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense;

public class LovenseStopPatterRequest {
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    // Optional: string or string array, null = all toys
    [JsonPropertyName("toy")]
    public object? Toy { get; set; }

    [JsonPropertyName("apiVer")]
    public int ApiVer { get; set; } = 1;
}