using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense.Request;

public class LovenseGetToysRequest {
    [JsonPropertyName("command")]
    public string Command { get; } = "GetToys";
}