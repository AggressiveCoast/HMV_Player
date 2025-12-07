using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense.Request;

public class LovenseFunctionRequest {
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// Action string format: e.g., "Vibrate10", "Rotate20", "All15", "Stop"
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Total running time in seconds. 0 = indefinite
    /// </summary>
    [JsonPropertyName("timeSec")]
    public double TimeSec { get; set; }

    /// <summary>
    /// Optional: Running time for loop
    /// </summary>
    [JsonPropertyName("loopRunningSec")]
    public double? LoopRunningSec { get; set; }

    /// <summary>
    /// Optional: Pause time between loops
    /// </summary>
    [JsonPropertyName("loopPauseSec")]
    public double? LoopPauseSec { get; set; }

    /// <summary>
    /// Optional: single string or array of toy IDs. Null = all toys
    /// </summary>
    [JsonPropertyName("toy")]
    public object? Toy { get; set; }

    /// <summary>
    /// Stop previous commands? Default 1 = stop previous
    /// </summary>
    [JsonPropertyName("stopPrevious")]
    public int StopPrevious { get; set; } = 1;

    [JsonPropertyName("apiVer")]
    public int ApiVer { get; set; } = 1;
}